using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Persistence.Repositories;

namespace CJTasksHelperBot.Persistence;

public class UnitOfWork : IUnitOfWork
{
	private bool _disposed;
	private Dictionary<Type, object>? _repositories;

	public UnitOfWork(ApplicationDbContext dbContext)
	{
		DbContext = dbContext;
	}

	public ApplicationDbContext DbContext { get; set; } 

	public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
	{
		_repositories ??= new Dictionary<Type, object>();

		var type = typeof(TEntity);
		if (!_repositories.ContainsKey(type))
		{
			_repositories[type] = new GenericRepository<TEntity>(DbContext);
		}

		return (IGenericRepository<TEntity>)_repositories[type];
	}

	public async Task<int> CommitAsync()
	{
		return await DbContext.SaveChangeAsync();
	}

	public void Dispose()
	{
		Dispose(true);

		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				_repositories?.Clear();

				DbContext.Dispose();
			}
		}

		_disposed = true;
	}
}