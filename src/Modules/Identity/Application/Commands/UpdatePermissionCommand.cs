using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Identity.Application.Commands;
public class UpdatePermissionCommand : IRequest<Result<PermissionDto>>
{
    public UpdatePermissionCommand(Guid permissionId, UpdatePermissionRequest request)
    {
        Request = request;
        PermissionId = permissionId;
    }
    public Guid PermissionId { get; set; }
    public UpdatePermissionRequest Request { get; set; }
    public class Handler : IRequestHandler<UpdatePermissionCommand, Result<PermissionDto>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<Permission>
            _permissionRepository;
        public Handler(
            IRepository<Permission>
                permissionRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _permissionRepository =
                permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Result<PermissionDto>> Handle(
            UpdatePermissionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating permission: {PermissionId}", request.PermissionId);
                var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
                if (permission == null)
                {
                    _logger.LogWarning("Permission not found: {PermissionId}", request.PermissionId);
                    return Result<PermissionDto>.Failure("Permission not found");
                }
                permission.UpdatePermission(request.Request.PermissionName, request.Request.Description);
                await _permissionRepository.UpdateAsync(permission, cancellationToken);
                await _permissionRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Permission updated successfully");
                return Result<PermissionDto>.Success(_mapper.Map<PermissionDto>(permission));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating permission");
                return Result<PermissionDto>.Failure("An unexpected error occurred while updating permission");
            }
        }
    }
}