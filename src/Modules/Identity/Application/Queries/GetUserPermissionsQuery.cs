using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries;

/// <summary>
/// Kullanýcýnýn tüm izinlerini al (roller üzerinden)
/// </summary>
public class GetUserPermissionsQuery : IRequest<Result<List<PermissionDto>>>
{
    public GetUserPermissionsQuery(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
    }

    public Guid UserId { get; set; }

    public class Handler : IRequestHandler<GetUserPermissionsQuery, Result<List<PermissionDto>>>
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

        public async Task<Result<List<PermissionDto>>> Handle(
            GetUserPermissionsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting permissions for user {UserId}", request.UserId);

                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<List<PermissionDto>>.Failure("User not found");
                }

                // Kullanýcýnýn rollerinden tüm izinleri topla
                var permissions = user.Roles
                    .Where(r => r.IsActive)
                    .SelectMany(r => r.Permissions)
                    .Distinct()
                    .ToList();

                var permissionDtos = _mapper.Map<List<PermissionDto>>(permissions);

                _logger.LogInformation("Retrieved {Count} permissions for user {UserId}",
                    permissionDtos.Count, request.UserId);

                return Result<List<PermissionDto>>.Success(permissionDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions for user {UserId}", request.UserId);
                return Result<List<PermissionDto>>.Failure("An error occurred while retrieving permissions");
            }
        }
    }
}