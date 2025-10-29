using Core.Application.Abstractions;

namespace Core.Application.Services;

internal class CurrentUserService : ICurrentUserService
{
    public CurrentUserService()
    {
        UserId = Guid.Empty;
        Username = "yakupeyisan";
        Email = "yakupeyisan@gmail.com";
        Roles = new List<string>();
        Permissions = new List<string>();
        DepartmentId = Guid.Empty;
        FacultyId = Guid.Empty;
        IsAdmin = true;
        IsAuthenticated = true;
    }

    public Guid UserId { get; }
    public string Username { get; }
    public string Email { get; }
    public IList<string> Roles { get; }
    public IList<string> Permissions { get; }
    public Guid? DepartmentId { get; }
    public Guid? FacultyId { get; }
    public bool IsAdmin { get; }
    public bool IsAuthenticated { get; }

    public bool IsInRole(string role)
    {
        return true;
    }

    public bool HasPermission(string permission)
    {
        return true;
    }

    public bool HasAnyRole(params string[] roles)
    {
        return true;
    }

    public bool HasAllRoles(params string[] roles)
    {
        return true;
    }
}