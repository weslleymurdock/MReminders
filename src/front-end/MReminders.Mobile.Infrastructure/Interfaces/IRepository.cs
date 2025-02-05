using System.Linq.Expressions;

namespace MReminders.Mobile.Infrastructure.Interfaces;

public interface IRepository <T> where T : class
{
    Task<(bool, T)> AddAsync(T entity);
    Task<bool> DeleteAsync(T entity);
    Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter);
    Task<IEnumerable<T>> GetAsync<U>(Expression<Func<T, bool>> filter, Expression<Func<T, U>> includes);
    Task<(bool, T)> UpdateAsync(T entity);
}
