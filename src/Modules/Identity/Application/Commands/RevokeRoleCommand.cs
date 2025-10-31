using AutoMapper;
using Core.Application.Abstractions;
using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
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
        private readonly ICurrentUserService _currentUserService;

        private readonly IRepository<User>
            _userRepository;

        public Handler(
            IRepository<User>
                userRepository,
            IMapper mapper,
            ILogger<Handler> logger, ICurrentUserService currentUserService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currentUserService = currentUserService;
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
                user.RemoveRole(request.Request.RoleId, _currentUserService.UserId);

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