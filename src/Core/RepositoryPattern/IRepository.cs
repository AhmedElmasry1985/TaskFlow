using System.Linq.Expressions;

namespace Core.RepositoryPattern;

public interface IRepository<TEntity> where TEntity : class
{
    Task Add(TEntity entity);
    Task AddRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
    Task<TEntity?> FindById(int id);
    Task <IEnumerable<TEntity>> FindByPredicate(Expression<Func<TEntity, bool>>? predicate = null);
}