namespace CJTasksHelperBot.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
	IChatRepository ChatRepository { get; }
	IUserRepository UserRepository { get; }
	IUserChatRepository UserChatRepository { get; }
	ITaskRepository TaskRepository { get; }
	IUserTaskStatusRepository UserTaskStatusRepository { get; }
	Task<int> CommitAsync();
}