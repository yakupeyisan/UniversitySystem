using Core.Domain.Results;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class DeleteUserCommand : IRequest<Result<bool>>
{
    public Guid UserId { get; set; }

    public DeleteUserCommand(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
    }

    public class Handler : IRequestHandler<DeleteUserCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IUserRepository userRepository,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<bool>> Handle(
            DeleteUserCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting user: {UserId}", request.UserId);

                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<bool>.Failure("User not found");
                }

                await _userRepository.DeleteAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User deleted successfully");
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return Result<bool>.Failure("An unexpected error occurred while deleting user");
            }
        }
    }
}