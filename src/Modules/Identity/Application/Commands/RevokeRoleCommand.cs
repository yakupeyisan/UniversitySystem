using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class RevokeRoleCommand : IRequest<Result<UserDto>>
{
    public RevokeRoleCommand(Guid userId, RevokeRoleRequest request)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public Guid UserId { get; set; }
    public RevokeRoleRequest Request { get; set; }

    public class Handler : IRequestHandler<RevokeRoleCommand, Result<UserDto>>
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
            RevokeRoleCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Attempting to revoke role {RoleId} from user {UserId}",
                    request.Request.RoleId,
                    request.UserId);

                // Get user
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<UserDto>.Failure("User not found");
                }

                // Remove role from user
                user.RemoveRole(request.Request.RoleId);

                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Role successfully revoked from user: {UserId}", request.UserId);

                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while revoking role from user: {UserId}", request.UserId);
                return Result<UserDto>.Failure("An unexpected error occurred while revoking role");
            }
        }
    }
}