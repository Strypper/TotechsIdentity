using Entities;
using System.Security.Claims;

namespace Repositories.Extensions
{
    public static class ProjectPermissionExtensions
    {
        public static Claim[] ClaimsExtensions(this ProjectPermission[] projectPermissions)
        {
            var claims = new Claim[] { };
            foreach (var projectPermission in projectPermissions)
            {
                new Claim(projectPermission.ProjectId, projectPermission.IsApproved.ToString());
            }
            return claims;
        }
    }
}
