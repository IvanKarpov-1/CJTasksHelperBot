using CJTasksHelperBot.Domain.Entities;

namespace CJTasksHelperBot.Application.Common.Interfaces;

public interface IUserTaskStatusRepository : IGenericRepository<Domain.Entities.UserTaskStatus>
{
    Task<List<Domain.Entities.UserTaskStatus>> GetTasksWithMissedDeadlinesAsync();
}