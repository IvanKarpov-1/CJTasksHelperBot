using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Persistence.Repositories;

public class UserTaskStatusRepository(ApplicationDbContext context)
    : GenericRepository<UserTaskStatus>(context), IUserTaskStatusRepository
{
    public async Task<List<UserTaskStatus>> GetTasksWithMissedDeadlinesAsync()
    {
        var taskStatuses = await ApplicationDbContext.UserTaskStatuses
            .Where(x =>
                x.Task != null &&
                x.Task.Deadline < DateTime.UtcNow &&
                new[]
                {
                    TaskStatus.AlmostDone,
                    TaskStatus.InProgress,
                    TaskStatus.NotStarted
                }.Contains(x.TaskStatus))
            .ToListAsync();

        return taskStatuses;
    }

    public async Task<UserTaskStatus?> GetUserTaskStatus(long userId, string partialTaskId, string partialTaskTitle)
    {
        partialTaskId = partialTaskId.ToUpper();

        return await ApplicationDbContext.UserTaskStatuses
            .Where(x => x.UserId == userId &&
                        x.TaskId.ToString().Contains(partialTaskId) &&
                        x.Task!.Title!.Contains(partialTaskTitle))
            .FirstOrDefaultAsync();
    }

    private ApplicationDbContext ApplicationDbContext => (Context as ApplicationDbContext)!;
}