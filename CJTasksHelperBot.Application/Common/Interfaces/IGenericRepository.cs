using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace CJTasksHelperBot.Application.Common.Interfaces;

public interface IGenericRepository<TEntity> where TEntity : class
{
	Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, bool tracking = true);
	Task<TEntity?> GetByApplicationIdAsync(Guid id);
	Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = true);
	Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate, bool tracking = true);
	void Add(TEntity entity);
	void Attach(TEntity entity);
	void Delete(TEntity entity);
	IQueryable<TEntity> GetQueryable();
}