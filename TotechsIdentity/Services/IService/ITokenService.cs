using Entities;
using System.Threading.Tasks;
using TotechsIdentity.AppSettings;

namespace TotechsIdentity.Services.IService
{
    public interface ITokenService
    {
        Task<string> GenerateToken(User user);
    }
}
