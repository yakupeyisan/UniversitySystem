using Core.Domain.Results;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class DeletePermissionCommand : IRequest<Result<bool>>
{
    public Guid PermissionId { get; set; }

    public DeletePermissionCommand(Guid permissionId)
    {
        if (permissionId == Guid.Empty)
            throw new ArgumentException("Permission ID cannot be empty", nameof(permissionId));

        PermissionId = permissionId;
    }

    public class Handler : IRequestHandler<DeletePermissionCommand, Result<bool>>
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IPermissionRepository permissionRepository,
            ILogger<Handler> logger)
        {
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<bool>> Handle(
            DeletePermissionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting permission: {PermissionId}", request.PermissionId);

                var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
                if (permission == null)
                {
                    _logger.LogWarning("Permission not found: {PermissionId}", request.PermissionId);
                    return Result<bool>.Failure("Permission not found");
                }

                await _permissionRepository.DeleteAsync(permission, cancellationToken);
                await _permissionRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Permission deleted successfully");
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting permission");
                return Result<bool>.Failure("An unexpected error occurred while deleting permission");
            }
        }
    }
}