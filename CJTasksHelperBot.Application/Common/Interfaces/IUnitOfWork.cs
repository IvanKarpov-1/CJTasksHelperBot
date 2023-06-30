namespace CJTasksHelperBot.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
	IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
	Task<int> CommitAsync();
}