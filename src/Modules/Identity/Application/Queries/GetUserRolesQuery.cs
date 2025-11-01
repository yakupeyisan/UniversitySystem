using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Identity.Application.Queries;
public class GetUserRolesQuery : IRequest<Result<List<RoleDto>>>
{
    public GetUserRolesQuery(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        UserId = userId;
    }
    public Guid UserId { get; set; }
    public class Handler : IRequestHandler<GetUserRolesQuery, Result<List<RoleDto>>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IRepository<User> _userRepository;
        private readonly AutoMapper.IMapper _mapper;
        public Handler(
            IRepository<User> userRepository,
            AutoMapper.IMapper mapper,
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
                _logger.LogInformation("Getting roles for user {UserId}", request.UserId);
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<List<RoleDto>>.Failure("User not found");
                }
                var roleDtos = _mapper.Map<List<RoleDto>>(user.Roles);
                _logger.LogInformation("Retrieved {Count} roles for user {UserId}",
                    roleDtos.Count, request.UserId);
                return Result<List<RoleDto>>.Success(roleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles for user {UserId}", request.UserId);
                return Result<List<RoleDto>>.Failure("An error occurred while retrieving roles");
            }
        }
    }
}