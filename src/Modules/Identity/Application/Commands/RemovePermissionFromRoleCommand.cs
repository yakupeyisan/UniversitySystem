using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class RemovePermissionFromRoleCommand : IRequest<Result<RoleDto>>
{
    public RemovePermissionFromRoleCommand(Guid roleId, Guid permissionId)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty", nameof(roleId));
        if (permissionId == Guid.Empty)
            throw new ArgumentException("Permission ID cannot be empty", nameof(permissionId));

        RoleId = roleId;
        PermissionId = permissionId;
    }

    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }

    public class Handler : IRequestHandler<RemovePermissionFromRoleCommand, Result<RoleDto>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;

        private readonly IRepository<Role>
            _roleRepository;

        public Handler(
            IRepository<Role>
                roleRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<RoleDto>> Handle(
            RemovePermissionFromRoleCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Removing permission {PermissionId} from role {RoleId}", request.PermissionId,
                    request.RoleId);

                var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
                if (role == null)
                {
                    _logger.LogWarning("Role not found: {RoleId}", request.RoleId);
                    return Result<RoleDto>.Failure("Role not found");
                }

                role.RemovePermission(request.PermissionId);
                await _roleRepository.UpdateAsync(role, cancellationToken);
                await _roleRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Permission removed from role successfully");
                return Result<RoleDto>.Success(_mapper.Map<RoleDto>(role));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing permission from role");
                return Result<RoleDto>.Failure("An unexpected error occurred while removing permission");
            }
        }
    }
}