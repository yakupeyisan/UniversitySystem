using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries;

public class ListRolesQuery : IRequest<Result<PaginatedListDto<RoleDto>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public ListRolesQuery(int pageNumber = 1, int pageSize = 10)
    {
        PageNumber = pageNumber > 0 ? pageNumber : 1;
        PageSize = pageSize > 0 ? pageSize : 10;
    }

    public class Handler : IRequestHandler<ListRolesQuery, Result<PaginatedListDto<RoleDto>>>
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

        public async Task<Result<PaginatedListDto<RoleDto>>> Handle(
            ListRolesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching roles - PageNumber: {PageNumber}, PageSize: {PageSize}",
                    request.PageNumber,
                    request.PageSize);

                var spec = new ActiveRolesSpecification(request.PageNumber, request.PageSize);

                var roles = await _roleRepository.GetBySpecificationAsync(spec, cancellationToken);
                var totalCount = await _roleRepository.GetCountAsync(new ActiveRolesSpecification(), cancellationToken);

                var roleDtos = _mapper.Map<List<RoleDto>>(roles);

                var result = new PaginatedListDto<RoleDto>
                {
                    Items = roleDtos,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                return Result<PaginatedListDto<RoleDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while listing roles");
                return Result<PaginatedListDto<RoleDto>>.Failure("An unexpected error occurred while listing roles");
            }
        }
    }
}