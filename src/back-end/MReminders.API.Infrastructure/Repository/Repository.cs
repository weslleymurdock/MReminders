using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MReminders.API.Infrastructure.Data;
using MReminders.API.Infrastructure.Interfaces;
using System.Linq.Expressions;

namespace MReminders.API.Infrastructure.Repository;

public class Repository<T>(AppDbContext context, ILogger<Repository<T>> logger) : IRepository<T> where T : class
{
    public async Task<T> CreateAsync(T entity)
    {
        try
        {
            var result = await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync();
            return result.Entity;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(T entity)
    {
        try
        {
            context.Set<T>().Remove(entity);
            return await context.SaveChangesAsync() > 0;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<bool> EditAsync(T entity)
    {
        try
        {
            context.Entry(entity).State = EntityState.Modified;
            return await context.SaveChangesAsync() > 0;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    public IEnumerable<T> Get()
    {
        try
        {
            return [.. context.Set<T>()];
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
    public IQueryable<T> Get(params Expression<Func<T, object>>[] includes)
    {
        try
        {
            IQueryable<T> query = context.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
    public T Get(Expression<Func<T, bool>> expression)
    {
        try
        {
            return context.Set<T>().Where(expression).FirstOrDefault() ?? default!;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
}
