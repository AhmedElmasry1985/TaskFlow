using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Core.RepositoryPattern;

public class Repository<TEntity> : IDisposable, IRepository<TEntity> where TEntity : class
{
    protected readonly DbContext _context;

    public Repository(DbContext context)
    {
        _context = context;
    }

    public async Task Add(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
    }

    public async Task AddRange(IEnumerable<TEntity> entities)
    {
        await _context.Set<TEntity>().AddRangeAsync(entities);
    }

    public void Remove(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        _context.Set<TEntity>().RemoveRange(entities);
    }

    public async Task<TEntity?> FindById(int id)
    {
        return await _context.Set<TEntity>().FindAsync(id);
    }

    public async Task<IEnumerable<TEntity>> FindByPredicate(Expression<Func<TEntity, bool>>? predicate)
    {
        return predicate == null ? 
            await _context.Set<TEntity>().ToListAsync() :
            await _context.Set<TEntity>().Where(predicate).ToListAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}