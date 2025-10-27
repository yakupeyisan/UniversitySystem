using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries;

#region Get Role By Id Query

public class GetRoleByIdQuery : IRequest<Result<RoleDto>>
{
    public Guid RoleId { get; set; }

    public GetRoleByIdQuery(Guid roleId)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty", nameof(roleId));

        RoleId = roleId;
    }

    public class Handler : IRequestHandler<GetRoleByIdQuery, Result<RoleDto>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IRoleRepository roleRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<RoleDto>> Handle(
            GetRoleByIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching role: {RoleId}", request.RoleId);

                var spec = new RoleByIdSpecification(request.RoleId);
                var role = await _roleRepository.GetAllAsync(spec, cancellationToken);

                if (role == null)
                {
                    _logger.LogWarning("Role not found: {RoleId}", request.RoleId);
                    return Result<RoleDto>.Failure("Role not found");
                }

                return Result<RoleDto>.Success(_mapper.Map<RoleDto>(role));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching role: {RoleId}", request.RoleId);
                return Result<RoleDto>.Failure("An unexpected error occurred while fetching role");
            }
        }
    }
}

#endregion

#region List Roles Query

public class ListRolesQuery : IRequest<Result<PaginatedListDto<RoleDto>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public ListRolesQuery(int pageNumber = 1, int pageSize = 10)
    {
        PageNumber = pageNumber > 0 ? pageNumber : 1;
        PageSize = pageSize > 0 ? pageSize : 10;
    }

    public class Handler : IRequestHandler<ListRolesQuery, Result<PaginatedListDto<RoleDto>>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IRoleRepository roleRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<PaginatedListDto<RoleDto>>> Handle(
            ListRolesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching roles - PageNumber: {PageNumber}, PageSize: {PageSize}",
                    request.PageNumber,
                    request.PageSize);

                var spec = new ActiveRolesSpecification();
                spec.ApplyPaging((request.PageNumber - 1) * request.PageSize, request.PageSize);

                var roles = await _roleRepository.GetAllAsync(spec, cancellationToken);
                var totalCount = await _roleRepository.GetCountAsync(new ActiveRolesSpecification(), cancellationToken);

                var roleDtos = _mapper.Map<List<RoleDto>>(roles);

                var result = new PaginatedListDto<RoleDto>
                {
                    Items = roleDtos,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                return Result<PaginatedListDto<RoleDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while listing roles");
                return Result<PaginatedListDto<RoleDto>>.Failure("An unexpected error occurred while listing roles");
            }
        }
    }
}

#endregion

#region Get Permission By Id Query

public class GetPermissionByIdQuery : IRequest<Result<PermissionDto>>
{
    public Guid PermissionId { get; set; }

    public GetPermissionByIdQuery(Guid permissionId)
    {
        if (permissionId == Guid.Empty)
            throw new ArgumentException("Permission ID cannot be empty", nameof(permissionId));

        PermissionId = permissionId;
    }

    public class Handler : IRequestHandler<GetPermissionByIdQuery, Result<PermissionDto>>
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IPermissionRepository permissionRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<PermissionDto>> Handle(
            GetPermissionByIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching permission: {PermissionId}", request.PermissionId);

                var spec = new PermissionByIdSpecification(request.PermissionId);
                var permission = await _permissionRepository.GetAllAsync(spec, cancellationToken);

                if (permission == null)
                {
                    _logger.LogWarning("Permission not found: {PermissionId}", request.PermissionId);
                    return Result<PermissionDto>.Failure("Permission not found");
                }

                return Result<PermissionDto>.Success(_mapper.Map<PermissionDto>(permission));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching permission: {PermissionId}", request.PermissionId);
                return Result<PermissionDto>.Failure("An unexpected error occurred while fetching permission");
            }
        }
    }
}

#endregion

#region List Permissions Query

public class ListPermissionsQuery : IRequest<Result<PaginatedListDto<PermissionDto>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public ListPermissionsQuery(int pageNumber = 1, int pageSize = 10)
    {
        PageNumber = pageNumber > 0 ? pageNumber : 1;
        PageSize = pageSize > 0 ? pageSize : 10;
    }

    public class Handler : IRequestHandler<ListPermissionsQuery, Result<PaginatedListDto<PermissionDto>>>
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IPermissionRepository permissionRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<PaginatedListDto<PermissionDto>>> Handle(
            ListPermissionsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching permissions - PageNumber: {PageNumber}, PageSize: {PageSize}",
                    request.PageNumber,
                    request.PageSize);

                var spec = new ActivePermissionsSpecification();
                spec.ApplyPaging((request.PageNumber - 1) * request.PageSize, request.PageSize);

                var permissions = await _permissionRepository.GetAllAsync(spec, cancellationToken);
                var totalCount = await _permissionRepository.GetCountAsync(new ActivePermissionsSpecification(), cancellationToken);

                var permissionDtos = _mapper.Map<List<PermissionDto>>(permissions);

                var result = new PaginatedListDto<PermissionDto>
                {
                    Items = permissionDtos,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                return Result<PaginatedListDto<PermissionDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while listing permissions");
                return Result<PaginatedListDto<PermissionDto>>.Failure("An unexpected error occurred while listing permissions");
            }
        }
    }
}

#endregion


#region Get User By Id Query

public class GetUserByIdQuery : IRequest<Result<UserDto>>
{
    public Guid UserId { get; set; }

    public GetUserByIdQuery(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
    }

    public class Handler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
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

        public async Task<Result<UserDto>> Handle(
            GetUserByIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching user: {UserId}", request.UserId);

                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

                if (user == null || user.IsDeleted)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<UserDto>.Failure("User not found");
                }

                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching user: {UserId}", request.UserId);
                return Result<UserDto>.Failure("An unexpected error occurred while fetching user");
            }
        }
    }
}

#endregion

#region Get User By Email Query

public class GetUserByEmailQuery : IRequest<Result<UserDto>>
{
    public string Email { get; set; }

    public GetUserByEmailQuery(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        Email = email;
    }

    public class Handler : IRequestHandler<GetUserByEmailQuery, Result<UserDto>>
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

        public async Task<Result<UserDto>> Handle(
            GetUserByEmailQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching user by email: {Email}", request.Email);

                var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

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

#endregion

#region List Users Query

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

                var spec = new ActiveUsersSpecification();
                spec.ApplyPaging((request.PageNumber - 1) * request.PageSize, request.PageSize);

                var users = await _userRepository.GetAllAsync(spec, cancellationToken);
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

#endregion

#region Search Users Query

public class SearchUsersQuery : IRequest<Result<PaginatedListDto<UserDto>>>
{
    public string SearchTerm { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public SearchUsersQuery(string searchTerm, int pageNumber = 1, int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            throw new ArgumentException("Search term cannot be empty", nameof(searchTerm));

        SearchTerm = searchTerm;
        PageNumber = pageNumber > 0 ? pageNumber : 1;
        PageSize = pageSize > 0 ? pageSize : 10;
    }

    public class Handler : IRequestHandler<SearchUsersQuery, Result<PaginatedListDto<UserDto>>>
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

                var spec = new UsersBySearchTermSpecification(request.SearchTerm);
                spec.ApplyPaging((request.PageNumber - 1) * request.PageSize, request.PageSize);

                var users = await _userRepository.GetAllAsync(spec, cancellationToken);
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

#endregion

#region Get User Roles Query

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

#endregion

#region Get User Permissions Query

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

#endregion


