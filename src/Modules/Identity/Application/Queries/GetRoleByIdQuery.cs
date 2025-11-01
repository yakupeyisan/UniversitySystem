using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Identity.Application.Queries;
public class GetRoleByIdQuery : IRequest<Result<RoleDto>>
{
    public GetRoleByIdQuery(Guid roleId)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty", nameof(roleId));
        RoleId = roleId;
    }
    public Guid RoleId { get; set; }
    public class Handler : IRequestHandler<GetRoleByIdQuery, Result<RoleDto>>
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
        public async Task<Result<RoleDto>> Handle(
            GetRoleByIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching role: {RoleId}", request.RoleId);
                var spec = new RoleByIdSpecification(request.RoleId);
                var role = await _roleRepository.GetAsync(spec, cancellationToken);
                if (role == null)
                {
                    _logger.LogWarning("Role not found: {RoleId}", request.RoleId);
                    return Result<RoleDto>.Failure("Role not found");
                }
                return Result<RoleDto>.Success(_mapper.Map<RoleDto>(role));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching role: {RoleId}", request.RoleId);
                return Result<RoleDto>.Failure("An unexpected error occurred while fetching role");
            }
        }
    }
}