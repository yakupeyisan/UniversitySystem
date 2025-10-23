using AutoMapper;
using Core.Application.Abstractions.Pagination;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Specifications;

namespace PersonMgmt.Application.Queries;

/// <summary>
/// Tüm kişileri getirme query (PAGINATION + NULLABLE FILTER İLE)
/// 
/// Kullanım:
/// 
/// 1. Filter olmadan:
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
/// 2. Filter ile:
/// var query = new GetAllPersonsQuery(
///     new PagedRequest { PageNumber = 1, PageSize = 20 },
///     "email|contains|@university.edu");
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
    /// Dinamik filter string (nullable)
    /// Format: field|operator|value;field2|operator2|value2
    /// Örnek: "email|contains|@university.edu;gender|eq|Male"
    /// </summary>
    public string? FilterString { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public GetAllPersonsQuery(PagedRequest pagedRequest, string? filterString = null)
    {
        PagedRequest = pagedRequest ?? throw new ArgumentNullException(nameof(pagedRequest));
        FilterString = filterString;
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
                    "Fetching all persons - Filter: {FilterString}, Page: {PageNumber}, Size: {PageSize}, Sort: {SortBy} {SortDirection}",
                    request.FilterString ?? "none",
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize,
                    request.PagedRequest.SortBy ?? "Default",
                    request.PagedRequest.SortDirection);

                // ✅ Specification kullanarak query oluştur
                var spec = new GetPersonsWithFiltersSpecification(
                    request.FilterString,
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize);

                var pagedList = await _personRepository.GetAllAsync(spec, request.PagedRequest, cancellationToken);

                var responses = _mapper.Map<List<PersonResponse>>(pagedList.Data);

                var result = new PagedList<PersonResponse>(
                    responses,
                    pagedList.TotalCount,
                    pagedList.PageNumber,
                    pagedList.PageSize);

                _logger.LogInformation(
                    "Retrieved persons successfully - Total: {TotalCount}, Returned: {ReturnedCount}, " +
                    "Page: {PageNumber}/{TotalPages}, Filter: {FilterString}",
                    result.TotalCount,
                    responses.Count,
                    result.PageNumber,
                    result.TotalPages,
                    request.FilterString ?? "none");

                return Result<PagedList<PersonResponse>>.Success(
                    result,
                    $"Persons retrieved successfully - {responses.Count} items on page {result.PageNumber} " +
                    $"({result.TotalCount} total)" +
                    (string.IsNullOrEmpty(request.FilterString) ? "" : $" with filter: {request.FilterString}"));
            }
            catch (Core.Domain.Filtering.FilterParsingException ex)
            {
                _logger.LogWarning(ex, "Filter parsing error: {FilterString}", request.FilterString);
                return Result<PagedList<PersonResponse>>.Failure($"Filter error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all persons - Filter: {FilterString}", request.FilterString);
                return Result<PagedList<PersonResponse>>.Failure(ex.Message);
            }
        }
    }
}