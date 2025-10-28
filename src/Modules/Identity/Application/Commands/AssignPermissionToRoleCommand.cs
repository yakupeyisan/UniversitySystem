using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class AssignPermissionToRoleCommand : IRequest<Result<RoleDto>>
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }

    public AssignPermissionToRoleCommand(Guid roleId, Guid permissionId)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty", nameof(roleId));
        if (permissionId == Guid.Empty)
            throw new ArgumentException("Permission ID cannot be empty", nameof(permissionId));

        RoleId = roleId;
        PermissionId = permissionId;
    }

    public class Handler : IRequestHandler<AssignPermissionToRoleCommand, Result<RoleDto>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<RoleDto>> Handle(
            AssignPermissionToRoleCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Assigning permission {PermissionId} to role {RoleId}", request.PermissionId, request.RoleId);

                var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
                if (role == null)
                {
                    _logger.LogWarning("Role not found: {RoleId}", request.RoleId);
                    return Result<RoleDto>.Failure("Role not found");
                }

                var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
                if (permission == null)
                {
                    _logger.LogWarning("Permission not found: {PermissionId}", request.PermissionId);
                    return Result<RoleDto>.Failure("Permission not found");
                }

                role.AddPermission(permission);
                await _roleRepository.UpdateAsync(role, cancellationToken);
                await _roleRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Permission assigned to role successfully");
                return Result<RoleDto>.Success(_mapper.Map<RoleDto>(role));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning permission to role");
                return Result<RoleDto>.Failure("An unexpected error occurred while assigning permission");
            }
        }
    }
}