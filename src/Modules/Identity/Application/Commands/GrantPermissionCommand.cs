using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class GrantPermissionCommand : IRequest<Result<UserDto>>
{
    public Guid UserId { get; set; }
    public GrantPermissionRequest Request { get; set; }

    public GrantPermissionCommand(Guid userId, GrantPermissionRequest request)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public class Handler : IRequestHandler<GrantPermissionCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IUserRepository userRepository,
            IPermissionRepository permissionRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<UserDto>> Handle(
            GrantPermissionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Attempting to grant permission {PermissionId} to user {UserId}",
                    request.Request.PermissionId,
                    request.UserId);

                // Get user
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<UserDto>.Failure("User not found");
                }

                // Get permission
                var permission = await _permissionRepository.GetByIdAsync(request.Request.PermissionId, cancellationToken);
                if (permission == null)
                {
                    _logger.LogWarning("Permission not found: {PermissionId}", request.Request.PermissionId);
                    return Result<UserDto>.Failure("Permission not found");
                }

                // Grant permission
                user.AddPermission(permission);

                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Permission successfully granted to user: {UserId}", request.UserId);

                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while granting permission to user: {UserId}", request.UserId);
                return Result<UserDto>.Failure("An unexpected error occurred while granting permission");
            }
        }
    }
}