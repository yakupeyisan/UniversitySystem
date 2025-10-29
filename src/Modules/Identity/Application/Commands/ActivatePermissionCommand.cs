using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class ActivatePermissionCommand : IRequest<Result<PermissionDto>>
{
    public ActivatePermissionCommand(Guid permissionId)
    {
        if (permissionId == Guid.Empty)
            throw new ArgumentException("Permission ID cannot be empty", nameof(permissionId));

        PermissionId = permissionId;
    }

    public Guid PermissionId { get; set; }

    public class Handler : IRequestHandler<ActivatePermissionCommand, Result<PermissionDto>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IPermissionRepository _permissionRepository;

        public Handler(
            IPermissionRepository permissionRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _permissionRepository =
                permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<PermissionDto>> Handle(
            ActivatePermissionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Activating permission: {PermissionId}", request.PermissionId);

                var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
                if (permission == null)
                {
                    _logger.LogWarning("Permission not found: {PermissionId}", request.PermissionId);
                    return Result<PermissionDto>.Failure("Permission not found");
                }

                permission.Activate();
                await _permissionRepository.UpdateAsync(permission, cancellationToken);
                await _permissionRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Permission activated successfully");
                return Result<PermissionDto>.Success(_mapper.Map<PermissionDto>(permission));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating permission");
                return Result<PermissionDto>.Failure("An unexpected error occurred while activating permission");
            }
        }
    }
}