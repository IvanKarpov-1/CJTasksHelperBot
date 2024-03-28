using System.Linq.Expressions;

namespace CJTasksHelperBot.Application.Common.Interfaces;

public interface IGenericRepository<TEntity> where TEntity : class
{
	Task<TEntity?> GetByApplicationIdAsync(Guid id);
	Task<List<TEntity>> GetAllAsync(bool tracking = true);
	Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, bool tracking = true);
	Task<List<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate, bool tracking = true);
	
	void Add(TEntity entity);
	void Attach(TEntity entity);
	void AddRange(IEnumerable<TEntity> entities);
	
	void Remove(TEntity entity);
	void RemoveRange(IEnumerable<TEntity> entities);
}