using System.Linq.Expressions;

namespace CJTasksHelperBot.Application.Common.Interfaces;

public interface IGenericRepository<TEntity> where TEntity : class
{
	Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate);
	Task<TEntity?> GetByApplicationIdAsync(Guid id);
	Task<IEnumerable<TEntity>> GetAllAsync();
	Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate);
	Task<System.Threading.Tasks.Task> AddAsync(TEntity entity);
	void Delete(TEntity entity);
}