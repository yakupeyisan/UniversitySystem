using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries;

public class GetRolePermissionsQuery : IRequest<Result<List<PermissionDto>>>
{
    public GetRolePermissionsQuery(Guid roleId)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty", nameof(roleId));

        RoleId = roleId;
    }

    public Guid RoleId { get; set; }

    public class Handler : IRequestHandler<GetRolePermissionsQuery, Result<List<PermissionDto>>>
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

        public async Task<Result<List<PermissionDto>>> Handle(
            GetRolePermissionsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching permissions for role: {RoleId}", request.RoleId);

                var spec = new RoleWithPermissionsSpecification(request.RoleId);
                var role = await _roleRepository.GetAsync(spec, cancellationToken);
                if (role == null)
                {
                    _logger.LogWarning("Role not found: {RoleId}", request.RoleId);
                    return Result<List<PermissionDto>>.Failure("Role not found");
                }

                var permissions = _mapper.Map<List<PermissionDto>>(role.Permissions);
                return Result<List<PermissionDto>>.Success(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching role permissions: {RoleId}", request.RoleId);
                return Result<List<PermissionDto>>.Failure("An unexpected error occurred while fetching permissions");
            }
        }
    }
}