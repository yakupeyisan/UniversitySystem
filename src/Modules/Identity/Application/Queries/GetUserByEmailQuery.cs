using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries;

public class GetUserByEmailQuery : IRequest<Result<UserDto>>
{
    public GetUserByEmailQuery(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        Email = email;
    }

    public string Email { get; set; }

    public class Handler : IRequestHandler<GetUserByEmailQuery, Result<UserDto>>
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

        public async Task<Result<UserDto>> Handle(
            GetUserByEmailQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching user by email: {Email}", request.Email);

                var user = await _userRepository.GetAsync(new UserByEmailSpecification(request.Email),
                    cancellationToken);

                if (user == null || user.IsDeleted)
                {
                    _logger.LogWarning("User not found with email: {Email}", request.Email);
                    return Result<UserDto>.Failure("User not found");
                }

                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching user by email: {Email}", request.Email);
                return Result<UserDto>.Failure("An unexpected error occurred while fetching user");
            }
        }
    }
}