using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class LogoutCommand : IRequest<Result<bool>>
{
    public LogoutCommand(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
    }

    public Guid UserId { get; set; }

    public class Handler : IRequestHandler<LogoutCommand, Result<bool>>
    {
        private readonly ILogger<Handler> _logger;

        private readonly IRepository<User>
            _userRepository;

        public Handler(
            IRepository<User>
                userRepository,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<bool>> Handle(
            LogoutCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Logging out user: {UserId}", request.UserId);

                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<bool>.Failure("User not found");
                }

                user.ClearRefreshToken();
                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User logged out successfully: {UserId}", request.UserId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging out user: {UserId}", request.UserId);
                return Result<bool>.Failure("An unexpected error occurred while logging out");
            }
        }
    }
}