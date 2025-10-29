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

public class ListRolesQuery : IRequest<Result<PagedList<RoleDto>>>
{
    public PagedRequest Request { get; private set; }

    public ListRolesQuery(PagedRequest request)
    {
        Request = request;
    }

    public class Handler : IRequestHandler<ListRolesQuery, Result<PagedList<RoleDto>>>
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

        public async Task<Result<PagedList<RoleDto>>> Handle(
            ListRolesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching roles - PageNumber: {PageNumber}, PageSize: {PageSize}",
                    request.Request.PageNumber,
                    request.Request.PageSize);

                var spec = new ActiveRolesSpecification();

                var roles = await _roleRepository.GetAllAsync(spec, request.Request, cancellationToken);

                var roleDtos = _mapper.Map<List<RoleDto>>(roles.Data);

                var result = new PagedList<RoleDto>(roleDtos, roles.TotalCount, roles.PageNumber, roles.PageSize);

                return Result<PagedList<RoleDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while listing roles");
                return Result<PagedList<RoleDto>>.Failure("An unexpected error occurred while listing roles");
            }
        }
    }
}