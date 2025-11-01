using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Identity.Application.Queries;
public class SearchPermissionsQuery : IRequest<Result<List<PermissionDto>>>
{
    public SearchPermissionsQuery(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            throw new ArgumentException("Search term cannot be empty", nameof(searchTerm));
        SearchTerm = searchTerm.Trim().ToLower();
    }
    public string SearchTerm { get; set; } = string.Empty;
    public class Handler : IRequestHandler<SearchPermissionsQuery, Result<List<PermissionDto>>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<Permission>
            _permissionRepository;
        public Handler(
            IRepository<Permission>
                permissionRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _permissionRepository =
                permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Result<List<PermissionDto>>> Handle(
            SearchPermissionsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Searching permissions with term: {SearchTerm}", request.SearchTerm);
                var spec = new SearchPermissionsSpecification(request.SearchTerm);
                var permissions = await _permissionRepository.GetAllAsync(spec, cancellationToken);
                if (!permissions.Any())
                {
                    _logger.LogWarning("No permissions found with search term: {SearchTerm}", request.SearchTerm);
                    return Result<List<PermissionDto>>.Success(new List<PermissionDto>());
                }
                var mappedPermissions = _mapper.Map<List<PermissionDto>>(permissions);
                return Result<List<PermissionDto>>.Success(mappedPermissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching permissions");
                return Result<List<PermissionDto>>.Failure("An unexpected error occurred while searching permissions");
            }
        }
    }
}