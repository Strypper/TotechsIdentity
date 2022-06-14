using Entities;
using System.Linq.Expressions;

namespace Contracts.TotechsIdentity
{
    public interface IBaseTotechRepository<T> where T : BaseEntity
    {
        IQueryable<T> FindAll(Expression<Func<T, bool>>? predicate = null);

        Task<T> FindByIdAsync(int id, CancellationToken cancelationToken);

        void Create(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task SaveChangesAsync(CancellationToken cancelationToken);
    }
}
