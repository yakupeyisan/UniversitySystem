using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries;

public class ListUsersQuery : IRequest<Result<PaginatedListDto<UserDto>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public ListUsersQuery(int pageNumber = 1, int pageSize = 10)
    {
        PageNumber = pageNumber > 0 ? pageNumber : 1;
        PageSize = pageSize > 0 ? pageSize : 10;
    }

    public class Handler : IRequestHandler<ListUsersQuery, Result<PaginatedListDto<UserDto>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

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
            ListUsersQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching users - PageNumber: {PageNumber}, PageSize: {PageSize}",
                    request.PageNumber,
                    request.PageSize);

                var spec = new ActiveUsersSpecification(request.PageNumber, request.PageSize);

                var users = await _userRepository.GetBySpecificationAsync(spec, cancellationToken);
                var totalCount = await _userRepository.GetCountAsync(new ActiveUsersSpecification(), cancellationToken);

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
                _logger.LogError(ex, "Unexpected error while listing users");
                return Result<PaginatedListDto<UserDto>>.Failure("An unexpected error occurred while listing users");
            }
        }
    }
}