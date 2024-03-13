using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.Task.Queries;
using CJTasksHelperBot.Infrastructure.Resources;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Task = System.Threading.Tasks.Task;

namespace CJTasksHelperBot.Infrastructure.Services.BackgroundServices;

public class SoonExpiredTasksNotifierService : BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromSeconds(1);
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
                var mediator = asyncScope.ServiceProvider.GetRequiredService<IMediator>();

                var (tasksPerGroup, tasksPerUser) = (await mediator.Send(new GetSoonExpiredTasksQuery(), stoppingToken)).Value;
                
                foreach (var grouping in tasksPerGroup)
                {
                    var groupId = grouping.Key;
                    var groupsTasks = grouping.ToList();

                    await NotifyGroupAsync(groupId, groupsTasks, stoppingToken);
                }

                foreach (var (userId, tasksPerChat) in tasksPerUser)
                {
                    await NotifyUserAsync(userId, tasksPerChat, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to execute {serviceName} with exception message: {message}", GetType(), ex.Message);
            }
        }
    }
    
    private async Task NotifyGroupAsync(long chatId, List<GetTaskDto> tasks, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
    
    private async Task NotifyUserAsync(long userId, Dictionary<string, List<GetTaskDto>> tasks, CancellationToken cancellationToken)
    {
        foreach (var tuple in tasks)
        {
            var title = tuple.Key == string.Empty
                ? _localizer["word_personal"]
                : tuple.Key;

            foreach (var task in tuple.Value)
            {
                try
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: userId,
                        text: $"{title}: {task.Title}",
                        cancellationToken: cancellationToken);
                }
                catch (Exception)
                {
                    // Ignore
                }
            }
        }
    }
}