using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Domain.GenericRepository
{
    public interface IRepository<TEntity>
    {
        Task<ICollection<TEntity>> ToListAsync();
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<int> InsertAsync(TEntity item);
        void Insert (TEntity item);
        Task InsertMultipleAsync(IEnumerable<TEntity> items);
        Task UpdateAsync(TEntity item);
        Task UpdateMultipleAsync(IEnumerable<TEntity> items);
        public Task RemoveMultipleByIdAsync(IEnumerable<int> ids);
        Task RemoveByIdAsync(int id);
        Task RemoveMultipleAsync(IEnumerable<TEntity> items);
    }
}
