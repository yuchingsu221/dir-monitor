using Domain.Domain.GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Models.Models;
using Models.Models.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DatabaseAccess.GenericRepository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly DIRContext context;
        private DbSet<TEntity> entities;

        public Repository(DIRContext context)
        {
            this.context = context;
            entities = context.Set<TEntity>();
        }

        public async Task<ICollection<TEntity>> ToListAsync()
        {
            return await entities.ToListAsync();
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await entities.FirstOrDefaultAsync(predicate);
        }

        public async Task<int> InsertAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            await entities.AddAsync(entity);
            // 上方的add async 並不會直接新增到entities
            var lastEntity = await entities.OrderBy(x => x.Id).LastOrDefaultAsync();
            return lastEntity is null ? 1 : lastEntity.Id + 1;
        }

        public  void Insert(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entities.Add(entity);
        }

        public async Task InsertMultipleAsync(IEnumerable<TEntity> newEntities)
        {
            if (newEntities == null) throw new ArgumentNullException("newEntities");

            await entities.AddRangeAsync(newEntities);
        }

        public async Task RemoveByIdAsync(int id)
        {
            TEntity entity = await entities.FirstOrDefaultAsync(s => s.Id == id);
            entities.Remove(entity);
        }

        public Task RemoveMultipleAsync(IEnumerable<TEntity> removeEntities)
        {
            if (removeEntities == null) throw new ArgumentNullException("removeEntities");
            entities.RemoveRange(removeEntities);
            return Task.CompletedTask;
        }

        public Task RemoveMultipleByIdAsync(IEnumerable<int> ids)
        {
            var removeEntities = entities.Where(e => ids.Contains(e.Id));
            entities.RemoveRange(removeEntities);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            entities.Update(entity);
            return Task.CompletedTask;
        }

        public Task UpdateMultipleAsync(IEnumerable<TEntity> newEntities)
        {
            if (newEntities == null) throw new ArgumentNullException("newEntities");
            entities.UpdateRange(newEntities);
            return Task.CompletedTask;
        }
    }
}
