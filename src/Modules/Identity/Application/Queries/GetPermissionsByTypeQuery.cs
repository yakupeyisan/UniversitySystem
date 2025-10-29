using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Enums;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries;

public class GetPermissionsByTypeQuery : IRequest<Result<List<PermissionDto>>>
{
    public GetPermissionsByTypeQuery(PermissionType permissionType)
    {
        PermissionType = permissionType;
    }

    public PermissionType PermissionType { get; set; }

    public class Handler : IRequestHandler<GetPermissionsByTypeQuery, Result<List<PermissionDto>>>
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

        public async Task<Result<List<PermissionDto>>> Handle(
            GetPermissionsByTypeQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching permissions by type: {PermissionType}", request.PermissionType);

                var spec = new PermissionsByTypeSpecification(request.PermissionType);
                var permissions = await _permissionRepository.GetAllAsync(spec, cancellationToken);
                if (!permissions.Any())
                {
                    _logger.LogWarning("No permissions found for type: {PermissionType}", request.PermissionType);
                    return Result<List<PermissionDto>>.Success(new List<PermissionDto>());
                }

                var mappedPermissions = _mapper.Map<List<PermissionDto>>(permissions);
                return Result<List<PermissionDto>>.Success(mappedPermissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching permissions by type: {PermissionType}", request.PermissionType);
                return Result<List<PermissionDto>>.Failure("An unexpected error occurred while fetching permissions");
            }
        }
    }
}