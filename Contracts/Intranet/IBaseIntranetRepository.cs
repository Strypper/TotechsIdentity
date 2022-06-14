using Entities;

namespace Contracts.Intranet
{
    public interface IBaseIntranetRepository<T> where T : BaseEntity
    {
        Task<T> CreateAsync<T>(string url, object o);
        Task<bool> CreateAsyncWithoutDTO<T>(string url, object o);
        Task<T> GetAsync<T>(string url);
        Task<T> GetByIdAsync<T>(string url, int id);
        Task<bool> RemoveAsync(string url, int id);
        Task<bool> UpdateAsync(string url, object o);
    }
}
