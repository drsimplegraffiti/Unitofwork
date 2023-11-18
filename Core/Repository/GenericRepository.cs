using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PocketBook.Core.IRepositories;
using PocketBook.Data;

namespace PocketBook.Core.Repository;

public class GenericRepository<T>: IGenericRepository<T> where T: class
{
    protected ApplicationDbContext context;
    internal DbSet<T> dbSet;
    public readonly ILogger _logger;

    public GenericRepository(ApplicationDbContext context,
        ILogger logger)
    {
        this.context = context;
        this.dbSet = context.Set<T>();
        _logger = logger;
    }


//virtual means that this method can be overridden in derived classes
// derived class are the classes that inherit from a base class
    public virtual async Task<IEnumerable<T>> All() 
    {
        return await dbSet.ToListAsync();
    }

    public virtual async Task<T> GetById(Guid id)
    {
        return await dbSet.FindAsync(id) ?? throw new Exception("Not found");
    }

    public virtual async Task<bool> Add(T entity)
    {
        await dbSet.AddAsync(entity);
        return await context.SaveChangesAsync() > 0; // if more than 0 rows are affected, return true
    }

    public virtual async Task<bool> Delete(Guid id)
    {
        var entity = await dbSet.FindAsync(id);
        if (entity == null)
        {
            throw new Exception("Not found");
        }

        dbSet.Remove(entity);
        return await context.SaveChangesAsync() > 0; // if more than 0 rows are affected, return true
    }

    public virtual async Task<bool> Upsert(T entity)
    {
        var existing = await dbSet.FindAsync(entity);
        if (existing == null)
        {
            await dbSet.AddAsync(entity);
        }
        else
        {
            context.Entry(existing).CurrentValues.SetValues(entity);
        }

        return await context.SaveChangesAsync() > 0;
    }

    public virtual async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate)
    {
        return await dbSet.Where(predicate).ToListAsync();
    }
}