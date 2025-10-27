using Core.Domain.Pagination;
using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
using Identity.Domain.Enums;

namespace Identity.Domain.Specifications;

public class UserByIdSpecification : Specification<User>
{
    public UserByIdSpecification(
        Guid id,
        int pageNumber,
        int pageSize) : this(id)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
    public UserByIdSpecification(Guid id)
    {
        Criteria = u => u.Id == id && !u.IsDeleted;
        AddInclude(u => u.Roles);
        AddInclude(u => u.Permissions);
    }
}

public class UserByEmailSpecification : Specification<User>
{
    public UserByEmailSpecification(string email)
    {
        Criteria = u => u.Email.Value == email.ToLower() && !u.IsDeleted;
        AddInclude(u => u.Roles);
        AddInclude(u => u.Permissions);
    }
}

public class ActiveUsersSpecification : Specification<User>
{
    public ActiveUsersSpecification(
        int pageNumber,
        int pageSize) : this()
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public ActiveUsersSpecification()
    {
        Criteria = u => !u.IsDeleted && u.Status == UserStatus.Active;
        AddOrderBy(u => u.FirstName);
        AddOrderBy(u => u.LastName);
    }
}

public class UsersByStatusSpecification : Specification<User>
{
    public UsersByStatusSpecification(
        UserStatus status,
        int pageNumber,
        int pageSize) : this(status)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public UsersByStatusSpecification(UserStatus status)
    {
        Criteria = u => !u.IsDeleted && u.Status == status;
        AddOrderBy(u => u.FirstName);
    }
}

public class UsersByRoleSpecification : Specification<User>
{
    public UsersByRoleSpecification(
        Guid roleId,
        int pageNumber,
        int pageSize) : this(roleId)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public UsersByRoleSpecification(Guid roleId)
    {
        Criteria = u => !u.IsDeleted && u.Roles.Any(r => r.Id == roleId);
        AddOrderBy(u => u.FirstName);
    }
}

public class LockedUsersSpecification : Specification<User>
{
    public LockedUsersSpecification(
        int pageNumber,
        int pageSize) : this()
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public LockedUsersSpecification()
    {
        Criteria = u => !u.IsDeleted && u.Status == UserStatus.Locked;
        AddOrderBy(u => u.FirstName);
    }
}

public class UnverifiedEmailUsersSpecification : Specification<User>
{
    public UnverifiedEmailUsersSpecification(
        int pageNumber,
        int pageSize) : this()
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public UnverifiedEmailUsersSpecification()
    {
        Criteria = u => !u.IsDeleted && !u.IsEmailVerified;
        AddOrderBy(u => u.CreatedAt);
    }
}

public class UsersBySearchTermSpecification : Specification<User>
{
    public UsersBySearchTermSpecification(
        string searchTerm,
        int pageNumber,
        int pageSize) : this(searchTerm)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public UsersBySearchTermSpecification(string searchTerm)
    {
        var term = searchTerm.ToLower();
        Criteria = u => !u.IsDeleted && (
            u.FirstName.ToLower().Contains(term) ||
            u.LastName.ToLower().Contains(term) ||
            u.Email.Value.Contains(term));

        AddOrderBy(u => u.FirstName);
    }
}

public class DeletedUsersSpecification : Specification<User>
{
    public DeletedUsersSpecification(
        int pageNumber,
        int pageSize) : this()
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public DeletedUsersSpecification()
    {
        Criteria = u => u.IsDeleted;
        AddOrderBy(u => u.DeletedAt);
    }
}


public class RoleByIdSpecification : Specification<Role>
{
    public RoleByIdSpecification(Guid id)
    {
        Criteria = r => r.Id == id;
        AddInclude(r => r.Permissions);
    }
}

public class RoleByNameSpecification : Specification<Role>
{
    public RoleByNameSpecification(string name)
    {
        Criteria = r => r.RoleName.ToLower() == name.ToLower();
        AddInclude(r => r.Permissions);
    }
}

public class RoleByTypeSpecification : Specification<Role>
{
    public RoleByTypeSpecification(RoleType roleType)
    {
        Criteria = r => r.RoleType == roleType;
        AddOrderBy(r => r.RoleName);
    }
}

public class ActiveRolesSpecification : Specification<Role>
{
    public ActiveRolesSpecification(
        int pageNumber,
        int pageSize) : this()
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
    public ActiveRolesSpecification()
    {
        Criteria = r => r.IsActive;
        AddOrderBy(r => r.RoleName);
    }
}

public class SystemRolesSpecification : Specification<Role>
{
    public SystemRolesSpecification(
        int pageNumber,
        int pageSize) : this()
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
    public SystemRolesSpecification()
    {
        Criteria = r => r.IsSystemRole;
        AddOrderBy(r => r.RoleName);
    }
}

public class PermissionByIdSpecification : Specification<Permission>
{
    public PermissionByIdSpecification(Guid id)
    {
        Criteria = p => p.Id == id;
    }
}

public class PermissionByNameSpecification : Specification<Permission>
{
    public PermissionByNameSpecification(string name)
    {
        Criteria = p => p.PermissionName.ToLower() == name.ToLower();
    }
}

public class PermissionByTypeSpecification : Specification<Permission>
{
    public PermissionByTypeSpecification(
        PermissionType type,
        int pageNumber,
        int pageSize) : this(type)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
    public PermissionByTypeSpecification(PermissionType type)
    {
        Criteria = p => p.PermissionType == type;
        AddOrderBy(p => p.PermissionName);
    }
}

public class ActivePermissionsSpecification : Specification<Permission>
{
    public ActivePermissionsSpecification(
        int pageNumber,
        int pageSize) : this()
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
    public ActivePermissionsSpecification()
    {
        Criteria = p => p.IsActive;
        AddOrderBy(p => p.PermissionName);
    }
}



