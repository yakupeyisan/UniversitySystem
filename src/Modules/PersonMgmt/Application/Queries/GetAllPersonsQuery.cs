using AutoMapper;
using Core.Domain.Pagination;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Interfaces;
using PersonMgmt.Domain.Specifications;
namespace PersonMgmt.Application.Queries;
public class GetAllPersonsQuery : IRequest<Result<PagedList<PersonResponse>>>
{
    public PagedRequest PagedRequest { get; set; }
    public string? FilterString { get; set; }
    public GetAllPersonsQuery(PagedRequest pagedRequest, string? filterString = null)
    {
        PagedRequest = pagedRequest ?? throw new ArgumentNullException(nameof(pagedRequest));
        FilterString = filterString;
    }
    public class Handler : IRequestHandler<GetAllPersonsQuery, Result<PagedList<PersonResponse>>>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;
        public Handler(IPersonRepository personRepository, IMapper mapper, ILogger<Handler> logger)
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