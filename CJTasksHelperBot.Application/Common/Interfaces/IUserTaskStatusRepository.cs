using CJTasksHelperBot.Domain.Entities;

namespace CJTasksHelperBot.Application.Common.Interfaces;

public interface IUserTaskStatusRepository : IGenericRepository<UserTaskStatus>
{
    Task<List<UserTaskStatus>> GetTasksWithMissedDeadlinesAsync();
}