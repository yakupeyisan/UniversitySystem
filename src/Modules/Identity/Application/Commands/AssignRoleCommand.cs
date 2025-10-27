using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class AssignRoleCommand : IRequest<Result<UserDto>>
{
    public Guid UserId { get; set; }
    public AssignRoleRequest Request { get; set; }

    public AssignRoleCommand(Guid userId, AssignRoleRequest request)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public class Handler : IRequestHandler<AssignRoleCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<UserDto>> Handle(
            AssignRoleCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Attempting to assign role {RoleId} to user {UserId}",
                    request.Request.RoleId,
                    request.UserId);

                // Get user
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<UserDto>.Failure("User not found");
                }

                // Get role
                var role = await _roleRepository.GetByIdAsync(request.Request.RoleId, cancellationToken);
                if (role == null)
                {
                    _logger.LogWarning("Role not found: {RoleId}", request.Request.RoleId);
                    return Result<UserDto>.Failure("Role not found");
                }

                // Add role to user
                user.AddRole(role);

                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Role {RoleName} successfully assigned to user {UserId}",
                    role.RoleName,
                    request.UserId);

                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while assigning role to user: {UserId}", request.UserId);
                return Result<UserDto>.Failure("An unexpected error occurred while assigning role");
            }
        }
    }
}