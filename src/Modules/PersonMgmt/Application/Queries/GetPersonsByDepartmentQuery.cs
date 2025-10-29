using AutoMapper;
using Core.Domain.Filtering;
using Core.Domain.Pagination;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Specifications;

namespace PersonMgmt.Application.Queries;

public class GetPersonsByDepartmentQuery : IRequest<Result<PagedList<PersonResponse>>>
{
    public GetPersonsByDepartmentQuery(
        Guid departmentId,
        PagedRequest pagedRequest,
        string? filterString = null)
    {
        DepartmentId = departmentId;
        PagedRequest = pagedRequest ?? throw new ArgumentNullException(nameof(pagedRequest));
        FilterString = filterString;
    }

    public Guid DepartmentId { get; set; }
    public PagedRequest PagedRequest { get; set; }
    public string? FilterString { get; set; }

    public class Handler : IRequestHandler<GetPersonsByDepartmentQuery, Result<PagedList<PersonResponse>>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;

        private readonly IRepository<Person>
            _personRepository;

        public Handler(IRepository<Person>
            personRepository, IMapper mapper, ILogger<Handler> logger)
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
                var spec = new GetPersonsWithFiltersSpecification(
                    request.FilterString,
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize);
                var pagedList = await _personRepository.GetAllAsync(spec, request.PagedRequest, cancellationToken);
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
            catch (FilterParsingException ex)
            {
                _logger.LogWarning(ex, "Filter parsing error: {FilterString}", request.FilterString);
                return Result<PagedList<PersonResponse>>.Failure($"Filter error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching persons by department - DepartmentId: {DepartmentId}, Filter: {FilterString}",
                    request.DepartmentId,
                    request.FilterString);
                return Result<PagedList<PersonResponse>>.Failure(ex.Message);
            }
        }
    }
}