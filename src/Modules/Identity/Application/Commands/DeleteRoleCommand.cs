using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class DeleteRoleCommand : IRequest<Result<bool>>
{
    public DeleteRoleCommand(Guid roleId)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty", nameof(roleId));

        RoleId = roleId;
    }

    /// <summary>
    ///     Silinecek rol ID'si
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    ///     Handler: Rol silme iþlemini gerçekleþtirir
    ///     Sorumluluklar:
    ///     - Repository'den rol getirme
    ///     - Sistem rolü kontrolü
    ///     - Rol silme
    ///     - Logging ve error handling
    /// </summary>
    public class Handler : IRequestHandler<DeleteRoleCommand, Result<bool>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IRoleRepository _roleRepository;

        public Handler(
            IRoleRepository roleRepository,
            ILogger<Handler> logger)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Rol silme iþlemini handle eder
        /// </summary>
        /// <param name="request">DeleteRoleCommand request'i</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Baþarýlý/baþarýsýz Result</returns>
        public async Task<Result<bool>> Handle(
            DeleteRoleCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to delete role: {RoleId}", request.RoleId);

                // Rol database'den getiriliyor
                var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);

                if (role == null)
                {
                    _logger.LogWarning("Role not found for deletion: {RoleId}", request.RoleId);
                    return Result<bool>.Failure("Role not found");
                }

                // Sistem rolü kontrolü - sistem rolleri silinemez
                if (role.IsSystemRole)
                {
                    _logger.LogWarning(
                        "Attempted to delete system role: {RoleId} ({RoleName})",
                        request.RoleId,
                        role.RoleName);
                    return Result<bool>.Failure("System roles cannot be deleted");
                }

                // Rol ve iliþkili izinleri silme
                await _roleRepository.DeleteAsync(role, cancellationToken);
                await _roleRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Role deleted successfully: {RoleId} ({RoleName})",
                    request.RoleId,
                    role.RoleName);

                return Result<bool>.Success(true);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation while deleting role: {RoleId}", request.RoleId);
                return Result<bool>.Failure($"Operation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting role: {RoleId}", request.RoleId);
                return Result<bool>.Failure("An unexpected error occurred while deleting role");
            }
        }
    }
}