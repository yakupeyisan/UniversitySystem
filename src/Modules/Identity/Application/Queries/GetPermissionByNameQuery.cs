using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries;

public class GetPermissionByNameQuery : IRequest<Result<PermissionDto>>
{
    public string PermissionName { get; set; } = string.Empty;

    public GetPermissionByNameQuery(string permissionName)
    {
        if (string.IsNullOrWhiteSpace(permissionName))
            throw new ArgumentException("Permission name cannot be empty", nameof(permissionName));

        PermissionName = permissionName.Trim();
    }

    public class Handler : IRequestHandler<GetPermissionByNameQuery, Result<PermissionDto>>
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IPermissionRepository permissionRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<PermissionDto>> Handle(
            GetPermissionByNameQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching permission by name: {PermissionName}", request.PermissionName);

                var spec = new PermissionByNameSpecification(request.PermissionName);
                var permission = await _permissionRepository.GetBySpecificationAsync(spec, cancellationToken);

                if (permission == null)
                {
                    _logger.LogWarning("Permission not found: {PermissionName}", request.PermissionName);
                    return Result<PermissionDto>.Failure("Permission not found");
                }

                return Result<PermissionDto>.Success(_mapper.Map<PermissionDto>(permission));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching permission by name: {PermissionName}", request.PermissionName);
                return Result<PermissionDto>.Failure("An unexpected error occurred while fetching permission");
            }
        }
    }
}