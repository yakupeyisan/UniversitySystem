using AutoMapper;
using Core.Domain.Pagination;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Specifications;

namespace PersonMgmt.Application.Queries;

/// <summary>
/// Tüm öğrencileri getirme query (PAGINATION + NULLABLE FILTER İLE)
/// 
/// Kullanım:
/// 
/// 1. Filter olmadan:
/// var query = new GetAllStudentsQuery(
///     new PagedRequest { PageNumber = 1, PageSize = 20 });
/// var result = await _mediator.Send(query);
/// 
/// 2. Filter ile:
/// var query = new GetAllStudentsQuery(
///     new PagedRequest { PageNumber = 1, PageSize = 20 },
///     "email|contains|@student.edu");
/// var result = await _mediator.Send(query);
/// </summary>
public class GetAllStudentsQuery : IRequest<Result<PagedList<PersonResponse>>>
{
    /// <summary>
    /// Sayfalama parametreleri
    /// </summary>
    public PagedRequest PagedRequest { get; set; }

    /// <summary>
    /// Dinamik filter string (nullable)
    /// Format: field|operator|value;field2|operator2|value2
    /// Örnek: "email|contains|@student.edu;gender|eq|Male"
    /// </summary>
    public string? FilterString { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public GetAllStudentsQuery(PagedRequest pagedRequest, string? filterString = null)
    {
        PagedRequest = pagedRequest ?? throw new ArgumentNullException(nameof(pagedRequest));
        FilterString = filterString;
    }

    /// <summary>
    /// GetAllStudentsQuery Handler
    /// </summary>
    public class Handler : IRequestHandler<GetAllStudentsQuery, Result<PagedList<PersonResponse>>>
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
            GetAllStudentsQuery request,
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
                    "Fetching all students - Filter: {FilterString}, Page: {PageNumber}, Size: {PageSize}",
                    request.FilterString ?? "none",
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize);

                // ✅ Specification kullanarak query oluştur
                var spec = new GetPersonsWithFiltersSpecification(
                    request.FilterString,
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize);

                var pagedList = await _personRepository.GetAllAsync(spec, request.PagedRequest, cancellationToken);

                // ✅ Sadece Student'leri filtrele
                var students = pagedList.Data
                    .Where(p => p.Student != null && !p.Student.IsDeleted)
                    .ToList();

                var responses = _mapper.Map<List<PersonResponse>>(students);

                var result = new PagedList<PersonResponse>(
                    responses,
                    students.Count,
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize);

                _logger.LogInformation(
                    "Retrieved students successfully - Total: {TotalCount}, Page: {PageNumber}/{TotalPages}, Filter: {FilterString}",
                    result.TotalCount,
                    result.PageNumber,
                    result.TotalPages,
                    request.FilterString ?? "none");

                return Result<PagedList<PersonResponse>>.Success(
                    result,
                    $"Students retrieved successfully - {responses.Count} items on page {result.PageNumber}" +
                    (string.IsNullOrEmpty(request.FilterString) ? "" : $" with filter: {request.FilterString}"));
            }
            catch (Core.Domain.Filtering.FilterParsingException ex)
            {
                _logger.LogWarning(ex, "Filter parsing error: {FilterString}", request.FilterString);
                return Result<PagedList<PersonResponse>>.Failure($"Filter error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching students - Filter: {FilterString}", request.FilterString);
                return Result<PagedList<PersonResponse>>.Failure(ex.Message);
            }
        }
    }
}