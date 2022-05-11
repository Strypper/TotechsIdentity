using Microsoft.AspNetCore.Authorization;
using Repositories;

namespace TotechsIdentity.Filters.Authorizations
{
    public sealed class AdministratorOnlyAttribute : AuthorizeAttribute
    {
        public AdministratorOnlyAttribute()
        {
            Roles = Repositories.Constants.Roles.Administrator;
        }
    }
}
