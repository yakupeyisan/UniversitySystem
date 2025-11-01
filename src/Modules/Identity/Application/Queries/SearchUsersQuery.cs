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
public class SearchUsersQuery : IRequest<Result<PagedList<UserDto>>>
{
    public SearchUsersQuery(string searchTerm, PagedRequest request)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            throw new ArgumentException("Search term cannot be empty", nameof(searchTerm));
        SearchTerm = searchTerm;
        Request = request;
    }
    public PagedRequest Request { get; private set; }
    public string SearchTerm { get; private set; }
    public class Handler : IRequestHandler<SearchUsersQuery, Result<PagedList<UserDto>>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<User>
            _userRepository;
        public Handler(
            IRepository<User>
                userRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Result<PagedList<UserDto>>> Handle(
            SearchUsersQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Searching users - SearchTerm: {SearchTerm}, PageNumber: {PageNumber}, PageSize: {PageSize}",
                    request.SearchTerm,
                    request.Request.PageNumber,
                    request.Request.PageSize);
                var spec = new UsersBySearchTermSpecification(request.SearchTerm);
                var users = await _userRepository.GetAllAsync(spec, request.Request, cancellationToken);
                var userDtos = _mapper.Map<List<UserDto>>(users.Data);
                var result = new PagedList<UserDto>(userDtos, users.TotalCount, users.PageNumber, users.PageSize);
                return Result<PagedList<UserDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while searching users");
                return Result<PagedList<UserDto>>.Failure("An unexpected error occurred while searching users");
            }
        }
    }
}