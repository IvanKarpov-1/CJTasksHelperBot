using CJTasksHelperBot.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CJTasksHelperBot.Persistence.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
	protected readonly DbContext Context;

	public GenericRepository(DbContext context)
	{
		Context = context;
	}

	public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, bool tracking = true)
	{
		return await GetQuery(tracking).FirstOrDefaultAsync(predicate);
	}

	public async Task<TEntity?> GetByApplicationIdAsync(Guid id)
	{
		return await Context.Set<TEntity>().FindAsync(id);
	}

	public async Task<List<TEntity>> GetAllAsync(bool tracking = true)
	{
		return await GetQuery(tracking).ToListAsync();
	}

	public async Task<List<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate, bool tracking = true)
	{
		return await GetQuery(tracking).Where(predicate).ToListAsync();
	}

	public void Add(TEntity entity)
	{
		try
		{
			Context.Set<TEntity>().Add(entity);
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
		Context.Set<TEntity>().Attach(entity);
	}

	public void AddRange(IEnumerable<TEntity> entities)
	{
		Context.Set<TEntity>().AddRange(entities);
	}

	public void Remove(TEntity entity)
	{
		Context.Set<TEntity>().Remove(entity);
	}

	public void RemoveRange(IEnumerable<TEntity> entities)
	{
		Context.Set<TEntity>().RemoveRange(entities);
	}

	private IQueryable<TEntity> GetQuery(bool tracking = false)
	{
		var query = Context.Set<TEntity>().AsQueryable();

		if (tracking == false) query = query.AsNoTracking();

		return query;
	}
}