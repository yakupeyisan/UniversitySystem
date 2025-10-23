using AutoMapper;
using Core.Application.Abstractions.Pagination;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;

namespace PersonMgmt.Application.Queries;

/// <summary>
/// Tüm kişileri getirme query (PAGINATION ILE)
/// 
/// Kullanım:
/// var query = new GetAllPersonsQuery(
///     new PagedRequest 
///     { 
///         PageNumber = 1, 
///         PageSize = 20, 
///         SortBy = "Name", 
///         SortDirection = "asc" 
///     });
/// var result = await _mediator.Send(query);
/// 
/// Response:
/// {
///   "success": true,
///   "data": {
///     "data": [ ... ],
///     "totalCount": 500,
///     "pageNumber": 1,
///     "pageSize": 20,
///     "totalPages": 25,
///     "hasNextPage": true,
///     "hasPreviousPage": false
///   }
/// }
/// </summary>
public class GetAllPersonsQuery : IRequest<Result<PagedList<PersonResponse>>>
{
    /// <summary>
    /// Sayfalama parametreleri
    /// </summary>
    public PagedRequest PagedRequest { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public GetAllPersonsQuery(PagedRequest pagedRequest)
    {
        PagedRequest = pagedRequest ?? throw new ArgumentNullException(nameof(pagedRequest));
    }

    /// <summary>
    /// GetAllPersonsQuery Handler
    /// </summary>
    public class Handler : IRequestHandler<GetAllPersonsQuery, Result<PagedList<PersonResponse>>>
    {
        private readonly IRepository<Person> _personRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(IRepository<Person> personRepository, IMapper mapper, ILogger<Handler> logger)
        {
            _personRepository = personRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PagedList<PersonResponse>>> Handle(
            GetAllPersonsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!request.PagedRequest.IsValid())
                {
                    var errorMsg = "Invalid pagination parameters";
                    _logger.LogWarning("Invalid pagination: PageNumber={PageNumber}, PageSize={PageSize}",
                        request.PagedRequest.PageNumber,
                        request.PagedRequest.PageSize);
                    return Result<PagedList<PersonResponse>>.Failure(errorMsg);
                }

                _logger.LogInformation(
                    "Fetching all persons - Page: {PageNumber}, Size: {PageSize}, Sort: {SortBy} {SortDirection}",
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize,
                    request.PagedRequest.SortBy ?? "Default",
                    request.PagedRequest.SortDirection);

                var allPersons = await _personRepository.GetAllAsync(cancellationToken);

                var filteredPersons = allPersons
                    .Where(p => !p.IsDeleted)
                    .ToList();

                var totalCount = filteredPersons.Count;


                var skip = request.PagedRequest.GetSkipCount();
                var pagedPersons = filteredPersons
                    .Skip(skip)
                    .Take(request.PagedRequest.PageSize)
                    .ToList();

                var responses = _mapper.Map<List<PersonResponse>>(pagedPersons);

                var pagedList = new PagedList<PersonResponse>(
                    responses,
                    totalCount,
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize);

                _logger.LogInformation(
                    "Retrieved persons successfully - Total: {TotalCount}, Page: {PageNumber}/{TotalPages}",
                    totalCount,
                    request.PagedRequest.PageNumber,
                    pagedList.TotalPages);

                return Result<PagedList<PersonResponse>>.Success(
                    pagedList,
                    $"Persons retrieved successfully - {pagedList.Data.Count} items on page {request.PagedRequest.PageNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all persons with pagination");
                return Result<PagedList<PersonResponse>>.Failure(ex.Message);
            }
        }
    }
}