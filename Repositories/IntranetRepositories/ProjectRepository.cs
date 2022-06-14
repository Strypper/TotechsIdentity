using Contracts.Intranet;
using Entities.IntranetEntities;
using System.Net.Http;

namespace Repositories.IntranetRepositories
{
    public class ProjectRepository : BaseIntranetRepository<Project>, IProjectRepository
    {
        public ProjectRepository(HttpClient hc) : base(hc) { }
    }
}
