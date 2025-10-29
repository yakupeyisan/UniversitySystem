using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class UpdateUserCommand : IRequest<Result<UserDto>>
{
    public UpdateUserCommand(Guid userId, UpdateUserRequest request)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public Guid UserId { get; set; }
    public UpdateUserRequest Request { get; set; }

    public class Handler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

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
            UpdateUserCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to update user: {UserId}", request.UserId);

                // Get user
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<UserDto>.Failure("User not found");
                }

                // Update profile
                user.UpdateProfile(request.Request.FirstName, request.Request.LastName);

                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User successfully updated: {UserId}", request.UserId);

                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Validation error while updating user: {UserId}", request.UserId);
                return Result<UserDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating user: {UserId}", request.UserId);
                return Result<UserDto>.Failure("An unexpected error occurred while updating user");
            }
        }
    }
}