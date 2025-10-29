using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class LockUserCommand : IRequest<Result<UserDto>>
{
    public LockUserCommand(Guid userId, string reason = "")
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
        Reason = reason?.Trim() ?? string.Empty;
    }

    public Guid UserId { get; set; }
    public string Reason { get; set; } = string.Empty;

    public class Handler : IRequestHandler<LockUserCommand, Result<UserDto>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;

        private readonly IRepository<User>
            _userRepository;

        public Handler(
            IRepository<User>
                userRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<UserDto>> Handle(
            LockUserCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Locking user: {UserId}", request.UserId);

                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<UserDto>.Failure("User not found");
                }

                user.LockAccount(request.Reason);
                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User locked successfully");
                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error locking user");
                return Result<UserDto>.Failure("An unexpected error occurred while locking user");
            }
        }
    }
}