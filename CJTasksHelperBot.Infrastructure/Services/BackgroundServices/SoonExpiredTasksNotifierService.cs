using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Infrastructure.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Task = System.Threading.Tasks.Task;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Infrastructure.Services.BackgroundServices;

public class SoonExpiredTasksNotifierService : BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromSeconds(15);
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SoonExpiredTasksNotifierService> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IStringLocalizer<Messages> _localizer;

    public SoonExpiredTasksNotifierService(IServiceProvider serviceProvider, ILogger<SoonExpiredTasksNotifierService> logger, ITelegramBotClient botClient, IStringLocalizer<Messages> localizer)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _botClient = botClient;
        _localizer = localizer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_period);
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await using var asyncScope = _serviceProvider.CreateAsyncScope();
                var unitOfWork = asyncScope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var tasks = await unitOfWork
                    .GetRepository<Domain.Entities.Task>()
                    .GetQueryable()
                    .Include(x => x.UserChat)
                    .ThenInclude(x => x!.Chat)
                    .Include(x => x.UserChat)
                    .ThenInclude(x => x!.User)
                    .Where(x => x.Deadline < DateTime.UtcNow.AddDays((int)x.NotificationLevel) &&
                        x.Deadline > DateTime.UtcNow &&
                           new[]
                           {
                               TaskStatus.NotStarted, 
                               TaskStatus.InProgress, 
                               TaskStatus.AlmostDone
                           }.Contains(x.Status) &&
                           x.NotificationLevel != NotificationLevel.Never)
                    .ToListAsync(stoppingToken);

                var groupedTasks = tasks
                    .GroupBy(x => new { x.UserChat!.User, x.UserChat.Chat })
                    .ToList();

                var tasksPerGroup = groupedTasks
                    .SelectMany(grouping => grouping
                        .Select(task => new { grouping.Key.Chat, Task = task }))
                    .GroupBy(item => item.Chat, item => item.Task)
                    .ToList();

                var tasksPerUser = groupedTasks
                    .SelectMany(grouping => grouping
                        .Select(task => new { grouping.Key.User, Task = task }))
                    .GroupBy(item => item.User, item => item.Task)
                    .ToList();

                foreach ( var grouping in tasksPerGroup)
                {
                    var group = grouping.Key;
                    var groupsTasks = grouping.ToList();

                    await NotifyGroupAsync(group!.Id, groupsTasks, stoppingToken);
                }

                foreach (var grouping in tasksPerUser)
                {
                    var user = grouping.Key;
                    var usersTasks = grouping.ToList();

                    var groupsTasks = new Dictionary<long, (string?, List<Domain.Entities.Task>)>();

                    foreach (var task in usersTasks)
                    {
                        if (groupsTasks.ContainsKey(task.UserChat!.ChatId) == false)
                            groupsTasks[task.UserChat.ChatId] = (task.UserChat.Chat!.Title, []);
                        groupsTasks[task.UserChat.ChatId].Item2.Add(task);
                    }

                    await NotifyUserAsync(user!.Id, groupsTasks, stoppingToken);
                }

                foreach (var task in tasks)
                {
                    if (task.Deadline < DateTime.UtcNow.AddDays((int)NotificationLevel.Day))
                        task.SetNotificationLevel(NotificationLevel.Never);
                    else if (task.Deadline < DateTime.UtcNow.AddDays((int)NotificationLevel.TwoDays))
                        task.SetNotificationLevel(NotificationLevel.Day);
                    else
                        task.SetNotificationLevel();
                }

                await unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Failed to execute {serviceName} with exception message: {message}", GetType(), ex.Message);
            }
        }
    }

    private async Task NotifyGroupAsync(long chatId, List<Domain.Entities.Task> tasks, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
    
    private async Task NotifyUserAsync(long userId, Dictionary<long, (string?, List<Domain.Entities.Task>)> tasks, CancellationToken cancellationToken)
    {
        foreach (var tuple in tasks)
        {
            var title = tuple.Value.Item1 ?? _localizer["word_personal"];

            foreach (var task in tuple.Value.Item2)
            {
                await _botClient.SendTextMessageAsync(
                    chatId: userId,
                    text: $"{title}: {task.Title}",
                    cancellationToken: cancellationToken);
            }
        }
    }
}