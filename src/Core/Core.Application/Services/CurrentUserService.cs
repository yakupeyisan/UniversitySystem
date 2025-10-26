using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Application.Abstractions;

namespace Core.Application.Services
{
    internal class CurrentUserService:ICurrentUserService
    {
        public Guid UserId { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
        public IList<string> Roles { get; private set; }
        public IList<string> Permissions { get; private set; }
        public Guid? DepartmentId { get; private set; }
        public Guid? FacultyId { get; private set; }
        public bool IsAdmin { get; private set; }
        public bool IsAuthenticated { get; private set; }

        public CurrentUserService()
        {
            UserId=Guid.Empty;
            Username="yakupeyisan";
            Email="yakupeyisan@gmail.com";
            Roles=new List<string>();
            Permissions=new List<string>();
            DepartmentId=Guid.Empty;
            FacultyId=Guid.Empty;
            IsAdmin=true;
            IsAuthenticated=true;

        }
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
}
