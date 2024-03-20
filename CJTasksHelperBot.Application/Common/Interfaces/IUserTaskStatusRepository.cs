namespace CJTasksHelperBot.Application.Common.Interfaces;

public interface IUserTaskStatusRepository : IGenericRepository<Domain.Entities.UserTaskStatus>
{
    Task<List<Domain.Entities.UserTaskStatus>> GetTasksWithMissedDeadlinesAsync();
    Task<Domain.Entities.UserTaskStatus?> GetUserTaskStatus(long userId, string partialTaskId, string partialTaskTitle);
}