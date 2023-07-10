using CJTasksHelperBot.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CJTasksHelperBot.Persistence.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
	private readonly ApplicationDbContext _context;

	public GenericRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, bool tracking = true)
	{
		return await GetQuery(tracking).FirstOrDefaultAsync(predicate);
	}

	public async Task<TEntity?> GetByApplicationIdAsync(Guid id)
	{
		return await _context.Set<TEntity>().FindAsync(id);
	}

	public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = true)
	{
		return await GetQuery(tracking).ToListAsync();
	}

	public async Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate, bool tracking = true)
	{
		return await GetQuery(tracking).Where(predicate).ToListAsync();
	}

	public void Add(TEntity entity)
	{
		try
		{
			_context.Set<TEntity>().Add(entity);
		}
		catch (Exception e)
		{
			Console.BackgroundColor = ConsoleColor.Red;
			Console.WriteLine($"{e}");
			Console.ResetColor();
		}
	}

	public void Attach(TEntity entity)
	{
		_context.Set<TEntity>().Attach(entity);
	}

	public void Delete(TEntity entity)
	{
		_context.Set<TEntity>().Remove(entity);
	}

	private IQueryable<TEntity> GetQuery(bool tracking = false)
	{
		var query = _context.Set<TEntity>().AsQueryable();

		if (tracking == false) query = query.AsNoTracking();

		return query;
	}
}