using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class UnlockUserCommand : IRequest<Result<UserDto>>
{
    public Guid UserId { get; set; }

    public UnlockUserCommand(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
    }

    public class Handler : IRequestHandler<UnlockUserCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<UserDto>> Handle(
            UnlockUserCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Unlocking user: {UserId}", request.UserId);

                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<UserDto>.Failure("User not found");
                }

                user.UnlockAccount();
                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User unlocked successfully");
                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlocking user");
                return Result<UserDto>.Failure("An unexpected error occurred while unlocking user");
            }
        }
    }
}