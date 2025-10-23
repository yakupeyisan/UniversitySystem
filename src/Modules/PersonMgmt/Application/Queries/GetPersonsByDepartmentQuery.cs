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
/// Departmana göre kişileri getirme query (PAGINATION + NULLABLE FILTER İLE)
/// 
/// Kullanım:
/// 
/// 1. Filter olmadan:
/// var query = new GetPersonsByDepartmentQuery(
///     departmentId,
///     new PagedRequest { PageNumber = 1, PageSize = 20 });
/// var result = await _mediator.Send(query);
/// 
/// 2. Filter ile:
/// var query = new GetPersonsByDepartmentQuery(
///     departmentId,
///     new PagedRequest { PageNumber = 1, PageSize = 20 },
///     "email|contains|@university.edu");
/// var result = await _mediator.Send(query);
/// </summary>
public class GetPersonsByDepartmentQuery : IRequest<Result<PagedList<PersonResponse>>>
{
    /// <summary>
    /// Departman ID
    /// </summary>
    public Guid DepartmentId { get; set; }

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
    public GetPersonsByDepartmentQuery(
        Guid departmentId,
        PagedRequest pagedRequest,
        string? filterString = null)
    {
        DepartmentId = departmentId;
        PagedRequest = pagedRequest ?? throw new ArgumentNullException(nameof(pagedRequest));
        FilterString = filterString;
    }

    /// <summary>
    /// GetPersonsByDepartmentQuery Handler
    /// </summary>
    public class Handler : IRequestHandler<GetPersonsByDepartmentQuery, Result<PagedList<PersonResponse>>>
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
            GetPersonsByDepartmentQuery request,
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
                    "Fetching persons by department - DepartmentId: {DepartmentId}, Filter: {FilterString}, Page: {PageNumber}, Size: {PageSize}",
                    request.DepartmentId,
                    request.FilterString ?? "none",
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize);

                // ✅ Specification kullanarak query oluştur
                var spec = new GetPersonsWithFiltersSpecification(
                    request.FilterString,
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize);

                var pagedList = await _personRepository.GetAllAsync(spec, request.PagedRequest, cancellationToken);

                // ✅ Departman ID'sine göre filtrele
                var personsByDepartment = pagedList.Data
                    .Where(p => p.DepartmentId == request.DepartmentId)
                    .ToList();

                var responses = _mapper.Map<List<PersonResponse>>(personsByDepartment);

                var result = new PagedList<PersonResponse>(
                    responses,
                    personsByDepartment.Count,
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize);

                _logger.LogInformation(
                    "Retrieved persons by department successfully - DepartmentId: {DepartmentId}, Total: {TotalCount}, Page: {PageNumber}/{TotalPages}, Filter: {FilterString}",
                    request.DepartmentId,
                    result.TotalCount,
                    result.PageNumber,
                    result.TotalPages,
                    request.FilterString ?? "none");

                return Result<PagedList<PersonResponse>>.Success(
                    result,
                    $"Persons from department retrieved successfully - {responses.Count} items on page {result.PageNumber}" +
                    (string.IsNullOrEmpty(request.FilterString) ? "" : $" with filter: {request.FilterString}"));
            }
            catch (Core.Domain.Filtering.FilterParsingException ex)
            {
                _logger.LogWarning(ex, "Filter parsing error: {FilterString}", request.FilterString);
                return Result<PagedList<PersonResponse>>.Failure($"Filter error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching persons by department - DepartmentId: {DepartmentId}, Filter: {FilterString}",
                    request.DepartmentId,
                    request.FilterString);
                return Result<PagedList<PersonResponse>>.Failure(ex.Message);
            }
        }
    }
}