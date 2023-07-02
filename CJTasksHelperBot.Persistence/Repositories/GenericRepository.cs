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

	public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate)
	{
		return await _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
	}

	public async Task<TEntity?> GetByApplicationIdAsync(Guid id)
	{
		return await _context.Set<TEntity>().FindAsync(id);
	}

	public async Task<IEnumerable<TEntity>> GetAllAsync()
	{
		return await _context.Set<TEntity>().ToListAsync();
	}

	public async Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate)
	{
		return await _context.Set<TEntity>().Where(predicate).ToListAsync();
	}

	public async Task<Task> AddAsync(TEntity entity)
	{
		await _context.Set<TEntity>().AddAsync(entity);
		return Task.CompletedTask;
	}

	public void Delete(TEntity entity)
	{
		_context.Set<TEntity>().Remove(entity);
	}
}