namespace CJTasksHelperBot.Application.Common.Interfaces;

public interface ITaskRepository : IGenericRepository<Domain.Entities.Task>
{
    Task<List<Domain.Entities.Task>> GetSoonExpiredTasksAsync();
    Task<List<Domain.Entities.Task>> GetTasksFromChatAsync(long chatId);
}