using System.Linq.Expressions;

namespace NationalTeamManager.Application.Common.Interfaces
{
    public interface IEntityRepository<TEntity>
        where TEntity : class
    {
        IQueryable<TEntity> Query(params Expression<Func<TEntity, object>>[] includes);

        Task<List<TEntity>> GetAllAsync(
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes
        );

        Task<TEntity?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes
        );

        Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
