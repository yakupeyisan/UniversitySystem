namespace Core.Application.Interfaces;


public interface ICurrentUserService
{
    Guid UserId { get; }
    string Username { get; }
    string Email { get; }
    IList<string> Roles { get; }
    IList<string> Permissions { get; }
    Guid? DepartmentId { get; }
    Guid? FacultyId { get; }
    bool IsAdmin { get; }
    bool IsAuthenticated { get; }

    bool IsInRole(string role);
    bool HasPermission(string permission);
    bool HasAnyRole(params string[] roles);
    bool HasAllRoles(params string[] roles);
}