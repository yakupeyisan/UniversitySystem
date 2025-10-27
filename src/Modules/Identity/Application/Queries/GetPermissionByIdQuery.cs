using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries;

public class GetPermissionByIdQuery : IRequest<Result<PermissionDto>>
{
    public Guid PermissionId { get; set; }

    public GetPermissionByIdQuery(Guid permissionId)
    {
        if (permissionId == Guid.Empty)
            throw new ArgumentException("Permission ID cannot be empty", nameof(permissionId));

        PermissionId = permissionId;
    }

    public class Handler : IRequestHandler<GetPermissionByIdQuery, Result<PermissionDto>>
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
            GetPermissionByIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching permission: {PermissionId}", request.PermissionId);

                var spec = new PermissionByIdSpecification(request.PermissionId);
                var permission = await _permissionRepository.GetBySpecificationAsync(spec, cancellationToken);

                if (permission == null)
                {
                    _logger.LogWarning("Permission not found: {PermissionId}", request.PermissionId);
                    return Result<PermissionDto>.Failure("Permission not found");
                }

                return Result<PermissionDto>.Success(_mapper.Map<PermissionDto>(permission));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching permission: {PermissionId}", request.PermissionId);
                return Result<PermissionDto>.Failure("An unexpected error occurred while fetching permission");
            }
        }
    }
}