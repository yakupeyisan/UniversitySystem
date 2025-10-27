using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries;

public class ListPermissionsQuery : IRequest<Result<PaginatedListDto<PermissionDto>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public ListPermissionsQuery(int pageNumber = 1, int pageSize = 10)
    {
        PageNumber = pageNumber > 0 ? pageNumber : 1;
        PageSize = pageSize > 0 ? pageSize : 10;
    }

    public class Handler : IRequestHandler<ListPermissionsQuery, Result<PaginatedListDto<PermissionDto>>>
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

        public async Task<Result<PaginatedListDto<PermissionDto>>> Handle(
            ListPermissionsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching permissions - PageNumber: {PageNumber}, PageSize: {PageSize}",
                    request.PageNumber,
                    request.PageSize);

                var spec = new ActivePermissionsSpecification(request.PageNumber, request.PageSize);

                var permissions = await _permissionRepository.GetBySpecificationAsync(spec, cancellationToken);
                var totalCount = await _permissionRepository.GetCountAsync(new ActivePermissionsSpecification(), cancellationToken);

                var permissionDtos = _mapper.Map<List<PermissionDto>>(permissions);

                var result = new PaginatedListDto<PermissionDto>
                {
                    Items = permissionDtos,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                return Result<PaginatedListDto<PermissionDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while listing permissions");
                return Result<PaginatedListDto<PermissionDto>>.Failure("An unexpected error occurred while listing permissions");
            }
        }
    }
}