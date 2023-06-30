using System.Linq.Expressions;
using CJTasksHelperBot.Application.Common.Interfaces;

namespace CJTasksHelperBot.Persistence.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
	private readonly ApplicationDbContext _context;

	public GenericRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate)
	{
		throw new NotImplementedException();
	}

	public Task<TEntity> GetByApplicationIdAsync(Guid id)
	{
		throw new NotImplementedException();
	}

	public Task<TEntity> GetByTelegramIdAsync(long id)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<TEntity>> GetAllAsync()
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate)
	{
		throw new NotImplementedException();
	}

	public Task Add(TEntity entity)
	{
		throw new NotImplementedException();
	}

	public Task Delete(TEntity entity)
	{
		throw new NotImplementedException();
	}
}