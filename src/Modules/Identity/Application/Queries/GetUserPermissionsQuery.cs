using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries;

public class GetUserPermissionsQuery : IRequest<Result<List<PermissionDto>>>
{
    public Guid UserId { get; set; }

    public GetUserPermissionsQuery(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
    }

    public class Handler : IRequestHandler<GetUserPermissionsQuery, Result<List<PermissionDto>>>
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

        public async Task<Result<List<PermissionDto>>> Handle(
            GetUserPermissionsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching permissions for user: {UserId}", request.UserId);

                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<List<PermissionDto>>.Failure("User not found");
                }

                var permissionDtos = _mapper.Map<List<PermissionDto>>(user.Permissions);
                return Result<List<PermissionDto>>.Success(permissionDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching user permissions: {UserId}", request.UserId);
                return Result<List<PermissionDto>>.Failure("An unexpected error occurred while fetching user permissions");
            }
        }
    }
}