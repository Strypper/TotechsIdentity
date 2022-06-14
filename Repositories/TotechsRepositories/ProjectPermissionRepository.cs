using Contracts.TotechsIdentity;
using Entities;

namespace Repositories.TotechsRepositories
{
    public class ProjectPermissionRepository : BaseToTechsRepository<ProjectPermission>, IProjectPermissionRepository
    {
        public ProjectPermissionRepository(IdentityContext identityContext) : base(identityContext){ }
    }
}
