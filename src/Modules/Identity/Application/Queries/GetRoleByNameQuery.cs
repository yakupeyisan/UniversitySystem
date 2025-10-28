using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries;

public class GetRoleByNameQuery : IRequest<Result<RoleDto>>
{
    public string RoleName { get; set; } = string.Empty;

    public GetRoleByNameQuery(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("Role name cannot be empty", nameof(roleName));

        RoleName = roleName.Trim();
    }

    public class Handler : IRequestHandler<GetRoleByNameQuery, Result<RoleDto>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IRoleRepository roleRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<RoleDto>> Handle(
            GetRoleByNameQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching role by name: {RoleName}", request.RoleName);

                var spec = new RoleByNameSpecification(request.RoleName);
                var role = await _roleRepository.GetBySpecificationAsync(spec, cancellationToken);

                if (role == null)
                {
                    _logger.LogWarning("Role not found: {RoleName}", request.RoleName);
                    return Result<RoleDto>.Failure("Role not found");
                }

                return Result<RoleDto>.Success(_mapper.Map<RoleDto>(role));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching role by name: {RoleName}", request.RoleName);
                return Result<RoleDto>.Failure("An unexpected error occurred while fetching role");
            }
        }
    }
}