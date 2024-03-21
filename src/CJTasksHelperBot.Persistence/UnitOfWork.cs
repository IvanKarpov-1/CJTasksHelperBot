using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Persistence.Repositories;

namespace CJTasksHelperBot.Persistence;

public class UnitOfWork : IUnitOfWork
{
	private readonly Lazy<IChatRepository> _chatRepository;
	private readonly Lazy<IUserRepository> _userRepository;
	private readonly Lazy<IUserChatRepository> _userChatRepository;
	private readonly Lazy<ITaskRepository> _taskRepository;
	private readonly Lazy<IUserTaskStatusRepository> _userTaskStatusRepository;
	private bool _disposed;

	public UnitOfWork(ApplicationDbContext dbContext)
	{
		DbContext = dbContext;
		_chatRepository = new Lazy<IChatRepository>(() => new ChatRepository(dbContext));
		_userRepository = new Lazy<IUserRepository>(() => new UserRepository(dbContext));
		_userChatRepository = new Lazy<IUserChatRepository>(() => new UserChatRepository(dbContext));
		_taskRepository = new Lazy<ITaskRepository>(() => new TaskRepository(dbContext));
		_userTaskStatusRepository = new Lazy<IUserTaskStatusRepository>(() => new UserTaskStatusRepository(dbContext));
	}

	private ApplicationDbContext DbContext { get; set; } 

	public IChatRepository ChatRepository => _chatRepository.Value;
	public IUserRepository UserRepository => _userRepository.Value;
	public IUserChatRepository UserChatRepository => _userChatRepository.Value;
	public ITaskRepository TaskRepository => _taskRepository.Value;
	public IUserTaskStatusRepository UserTaskStatusRepository => _userTaskStatusRepository.Value;

	public async Task<int> CommitAsync()
	{
		try
		{
			return await DbContext.SaveChangeAsync();
		}
		catch (Exception e)
		{
			Console.BackgroundColor = ConsoleColor.Red;
			Console.WriteLine(e);
			Console.ResetColor();
			return await Task.FromResult(-1);
		}
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
				DbContext.Dispose();
			}
		}

		_disposed = true;
	}
}