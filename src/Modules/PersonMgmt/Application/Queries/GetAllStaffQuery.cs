using AutoMapper;
using Core.Domain.Pagination;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Interfaces;
using PersonMgmt.Domain.Specifications;
using System;
namespace PersonMgmt.Application.Queries;
public class GetAllStaffQuery : IRequest<Result<PagedList<PersonResponse>>>
{
    public PagedRequest PagedRequest { get; set; }
    public string? FilterString { get; set; }
    public GetAllStaffQuery(PagedRequest pagedRequest, string? filterString = null)
    {
        PagedRequest = pagedRequest ?? throw new ArgumentNullException(nameof(pagedRequest));
        FilterString = filterString;
    }
    public class Handler : IRequestHandler<GetAllStaffQuery, Result<PagedList<PersonResponse>>>
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
            GetAllStaffQuery request,
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
                    "Fetching all staff - Filter: {FilterString}, Page: {PageNumber}, Size: {PageSize}",
                    request.FilterString ?? "none",
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize);
                var spec = new GetPersonsWithFiltersSpecification(
                    request.FilterString,
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize);
                var pagedList = await _personRepository.GetAllAsync(spec, request.PagedRequest, cancellationToken);
                var staffPersons = pagedList.Data
                    .Where(p => p.Staff != null && !p.Staff.IsDeleted)
                    .ToList();
                var responses = _mapper.Map<List<PersonResponse>>(staffPersons);
                var result = new PagedList<PersonResponse>(
                    responses,
                    staffPersons.Count,
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize);
                _logger.LogInformation(
                    "Retrieved staff successfully - Total: {TotalCount}, Page: {PageNumber}/{TotalPages}, Filter: {FilterString}",
                    result.TotalCount,
                    result.PageNumber,
                    result.TotalPages,
                    request.FilterString ?? "none");
                return Result<PagedList<PersonResponse>>.Success(
                    result,
                    $"Staff members retrieved successfully - {responses.Count} items on page {result.PageNumber}" +
                    (string.IsNullOrEmpty(request.FilterString) ? "" : $" with filter: {request.FilterString}"));
            }
            catch (Core.Domain.Filtering.FilterParsingException ex)
            {
                _logger.LogWarning(ex, "Filter parsing error: {FilterString}", request.FilterString);
                return Result<PagedList<PersonResponse>>.Failure($"Filter error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching staff - Filter: {FilterString}", request.FilterString);
                return Result<PagedList<PersonResponse>>.Failure(ex.Message);
            }
        }
    }
}