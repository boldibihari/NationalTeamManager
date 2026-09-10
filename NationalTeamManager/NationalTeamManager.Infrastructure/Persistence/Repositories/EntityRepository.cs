using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NationalTeamManager.Application.Interfaces;
using NationalTeamManager.Domain.Interfaces;

namespace NationalTeamManager.Infrastructure.Persistence.Repositories
{
    public class EntityRepository<TEntity>(NationalTeamManagerDbContext dbContext)
        : IEntityRepository<TEntity>
        where TEntity : class, IEntity
    {
        private readonly DbSet<TEntity> dbSet = dbContext.Set<TEntity>();

        public async Task<List<TEntity>> GetAllAsync(
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes
        )
        {
            IQueryable<TEntity> query = dbSet.AsNoTracking();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes
        )
        {
            IQueryable<TEntity> query = dbSet.AsNoTracking();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await dbSet.AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            dbSet.Update(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await dbSet.FindAsync([id], cancellationToken);

            if (entity is null)
                return;

            dbSet.Remove(entity);

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
