using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Task = CJTasksHelperBot.Domain.Entities.Task;

namespace CJTasksHelperBot.Persistence.Repositories;

public class TaskRepository(ApplicationDbContext context) : GenericRepository<Task>(context), ITaskRepository
{
    public async Task<List<Task>> GetSoonExpiredTasksAsync()
    {
        var tasks = await ApplicationDbContext.Tasks
            .Include(x => x.UserChat)
            .ThenInclude(x => x!.Chat)
            .Include(x => x.UserChat)
            .ThenInclude(x => x!.User)
            .Include(x => x.UserTaskStatuses)
            .Where(
                x => x.Deadline < DateTime.UtcNow.AddDays((int)x.NotificationLevel) &&
                     x.Deadline > DateTime.UtcNow &&
                     x.NotificationLevel != NotificationLevel.Never)
            .ToListAsync();

        return tasks;
    }

    public async Task<List<Task>> GetTasksFromChatAsync(long chatId)
    {
        var tasks = await ApplicationDbContext.Tasks
            .Include(x => x.UserTaskStatuses)
            .Include(x => x.UserChat)
            .Where(x => x.UserChat!.ChatId == chatId)
            .ToListAsync();

        return tasks;
    }

    private ApplicationDbContext ApplicationDbContext => (Context as ApplicationDbContext)!;
}