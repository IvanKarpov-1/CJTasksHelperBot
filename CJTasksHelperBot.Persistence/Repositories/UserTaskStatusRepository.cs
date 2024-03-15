using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Task = CJTasksHelperBot.Domain.Entities.Task;

namespace CJTasksHelperBot.Persistence.Repositories;

public class UserTaskStatusRepository : GenericRepository<UserTaskStatus>, IUserTaskStatusRepository
{
    public UserTaskStatusRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<UserTaskStatus>> GetTasksWithMissedDeadlinesAsync()
    {
        var taskStatuses = await ApplicationDbContext.UserTaskStatuses
            .Where(x =>
                x.Task != null &&
                x.Task.Deadline < DateTime.UtcNow &&
                new[]
                {
                    Domain.Enums.TaskStatus.AlmostDone,
                    Domain.Enums.TaskStatus.InProgress,
                    Domain.Enums.TaskStatus.NotStarted
                }.Contains(x.TaskStatus))
            .ToListAsync();

        return taskStatuses;
    }

    private ApplicationDbContext ApplicationDbContext => (Context as ApplicationDbContext)!;
}