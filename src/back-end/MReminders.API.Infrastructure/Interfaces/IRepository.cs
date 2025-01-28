using System.Linq.Expressions;

namespace MReminders.API.Infrastructure.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T> CreateAsync(T entity);
    Task<bool> EditAsync(T entity);
    Task<bool> DeleteAsync(T entity);
    IEnumerable<T> Get();
    IQueryable<T> Get(params Expression<Func<T, object>>[] includes);
    T Get(Expression<Func<T,bool>> expression);
}
