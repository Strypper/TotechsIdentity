using Contracts.TotechsIdentity;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Repositories.TotechsRepositories
{
    public class BaseToTechsRepository<T> : IBaseTotechRepository<T> where T : BaseEntity
    {
        protected readonly IdentityContext _identityContext;
        protected readonly DbSet<T> _dbSet;
        public BaseToTechsRepository(IdentityContext identityContext)
        {
            _identityContext = identityContext;
            _dbSet = _identityContext.Set<T>();
        }
        public virtual IQueryable<T> FindAll(Expression<Func<T, bool>>? predicate = null)
            => _dbSet.WhereIf(predicate != null, predicate!);

        public virtual async Task<T> FindByIdAsync(int id, CancellationToken cancellationToken)
        {
            var item = await _identityContext.FindAsync<T>(new object[] { id }, cancellationToken);
            return item;
        }

        public void Create(T entity)
        {
            _dbSet.Add(entity);
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
            => _identityContext.SaveChangesAsync(cancellationToken);
    }
}
