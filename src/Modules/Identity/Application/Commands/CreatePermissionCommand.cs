using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class CreatePermissionCommand : IRequest<Result<PermissionDto>>
{
    public CreatePermissionCommand(CreatePermissionRequest request)
    {
        Request = request;
    }

    public CreatePermissionRequest Request { get; set; }


    public class Handler : IRequestHandler<CreatePermissionCommand, Result<PermissionDto>>
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
            CreatePermissionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating new permission: {PermissionName}", request.Request.PermissionName);

                var permission = Permission.Create(request.Request.PermissionName,
                    (PermissionType)request.Request.PermissionType, request.Request.Description);
                await _permissionRepository.AddAsync(permission, cancellationToken);
                await _permissionRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Permission created successfully: {PermissionId}", permission.Id);
                return Result<PermissionDto>.Success(_mapper.Map<PermissionDto>(permission));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating permission");
                return Result<PermissionDto>.Failure("An unexpected error occurred while creating permission");
            }
        }
    }
}