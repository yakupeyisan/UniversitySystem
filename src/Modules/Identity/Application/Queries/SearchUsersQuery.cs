using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries;

public class SearchUsersQuery : IRequest<Result<PaginatedListDto<UserDto>>>
{
    public SearchUsersQuery(string searchTerm, int pageNumber = 1, int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            throw new ArgumentException("Search term cannot be empty", nameof(searchTerm));

        SearchTerm = searchTerm;
        PageNumber = pageNumber > 0 ? pageNumber : 1;
        PageSize = pageSize > 0 ? pageSize : 10;
    }

    public string SearchTerm { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public class Handler : IRequestHandler<SearchUsersQuery, Result<PaginatedListDto<UserDto>>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public Handler(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<PaginatedListDto<UserDto>>> Handle(
            SearchUsersQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Searching users - SearchTerm: {SearchTerm}, PageNumber: {PageNumber}, PageSize: {PageSize}",
                    request.SearchTerm,
                    request.PageNumber,
                    request.PageSize);

                var spec = new UsersBySearchTermSpecification(request.SearchTerm, request.PageNumber, request.PageSize);

                var users = await _userRepository.GetAsync(spec, cancellationToken);
                var totalCount = await _userRepository.GetCountAsync(
                    new UsersBySearchTermSpecification(request.SearchTerm),
                    cancellationToken);

                var userDtos = _mapper.Map<List<UserDto>>(users);

                var result = new PaginatedListDto<UserDto>
                {
                    Items = userDtos,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                return Result<PaginatedListDto<UserDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while searching users");
                return Result<PaginatedListDto<UserDto>>.Failure("An unexpected error occurred while searching users");
            }
        }
    }
}