using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Infrastructure.Data;
using MReminders.Mobile.Infrastructure.Interfaces;
using System.Linq.Expressions;

namespace MReminders.Mobile.Infrastructure.Repositories;

public class Repository<T>(AppDbContext context, ILogger<Repository<T>> logger) : IRepository<T> where T : class
{
    public async Task<(bool, T)> AddAsync(T entity)
    {
        try
        {
            logger.LogInformation($"{nameof(AddAsync)} starts");
            var entry = await context.AddAsync(entity);

            return (await context.SaveChangesAsync() > 0, entry.Entity);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
        finally 
        {
            logger.LogInformation($"{nameof(AddAsync)} ends");
        }
    }

    public async Task<bool> DeleteAsync(T entity)
    {
        try
        {
            logger.LogInformation($"{nameof(DeleteAsync)} starts");
            context.Set<T>().Remove(entity);
            return await context.SaveChangesAsync() > 0;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
        finally
        {
            logger.LogInformation($"{nameof(DeleteAsync)} ends");
        }
    }

    public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter)
    {
        try
        {
            logger.LogInformation($"{nameof(GetAsync)} starts");
            return await context.Set<T>().Where(filter).ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
        finally
        {
            logger.LogInformation($"{nameof(GetAsync)} ends");
        }
    }

    public async Task<IEnumerable<T>> GetAsync<U>(Expression<Func<T, bool>> filter, Expression<Func<T, U>> includes)
    {
        try
        {
            logger.LogInformation($"{nameof(GetAsync)} starts");
            return await context.Set<T>().Include(includes).Where(filter).ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
        finally
        {
            logger.LogInformation($"{nameof(GetAsync)} ends");
        }
    }

    public async Task<(bool, T)> UpdateAsync(T entity)
    {
        try
        {
            logger.LogInformation($"{nameof(UpdateAsync)} starts");
            context.Set<T>().Entry(entity).State = EntityState.Modified;

            return (await context.SaveChangesAsync() > 0, entity);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
        finally
        {
            logger.LogInformation($"{nameof(UpdateAsync)} ends");
        }
    }
}
