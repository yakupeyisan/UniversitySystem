using AutoMapper;
using Core.Domain.Pagination;
using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries;

public class ListPermissionsQuery : IRequest<Result<PagedList<PermissionDto>>>
{
    public PagedRequest Request { get; private set; }

    public ListPermissionsQuery(PagedRequest request)
    {
        Request = request;
    }

    public class Handler : IRequestHandler<ListPermissionsQuery, Result<PagedList<PermissionDto>>>
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

        public async Task<Result<PagedList<PermissionDto>>> Handle(
            ListPermissionsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching permissions - PageNumber: {PageNumber}, PageSize: {PageSize}",
                    request.Request.PageNumber,
                    request.Request.PageSize);

                var spec = new ActivePermissionsSpecification();

                var permissions = await _permissionRepository.GetAllAsync(spec, request.Request, cancellationToken);

                var permissionDtos = _mapper.Map<List<PermissionDto>>(permissions.Data);

                var result = new PagedList<PermissionDto>(permissionDtos, permissions.TotalCount,
                    permissions.PageNumber, permissions.PageSize);

                return Result<PagedList<PermissionDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while listing permissions");
                return Result<PagedList<PermissionDto>>.Failure(
                    "An unexpected error occurred while listing permissions");
            }
        }
    }
}