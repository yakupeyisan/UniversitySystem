using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries;

public class GetUserRolesQuery : IRequest<Result<List<RoleDto>>>
{
    public Guid UserId { get; set; }

    public GetUserRolesQuery(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
    }

    public class Handler : IRequestHandler<GetUserRolesQuery, Result<List<RoleDto>>>
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

        public async Task<Result<List<RoleDto>>> Handle(
            GetUserRolesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching roles for user: {UserId}", request.UserId);

                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<List<RoleDto>>.Failure("User not found");
                }

                var roleDtos = _mapper.Map<List<RoleDto>>(user.Roles);
                return Result<List<RoleDto>>.Success(roleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching user roles: {UserId}", request.UserId);
                return Result<List<RoleDto>>.Failure("An unexpected error occurred while fetching user roles");
            }
        }
    }
}