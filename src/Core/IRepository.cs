using System.Linq.Expressions;

namespace Core;

public interface IRepository<TEntity> where TEntity : class
{
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
    TEntity? Find(int id);
    IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
}