using System.Text;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.Task.Queries;
using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Infrastructure.Common.Extensions;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Resources;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Task = System.Threading.Tasks.Task;

namespace CJTasksHelperBot.Infrastructure.Services.BackgroundServices;

public class SoonExpiredTasksNotifierService : BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromHours(8);
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SoonExpiredTasksNotifierService> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly IDataPresentationService _dataPresentationService;

    public SoonExpiredTasksNotifierService(IServiceProvider serviceProvider, ILogger<SoonExpiredTasksNotifierService> logger, ITelegramBotClient botClient, IStringLocalizer<Messages> localizer, IDataPresentationService dataPresentationService)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _botClient = botClient;
        _localizer = localizer;
        _dataPresentationService = dataPresentationService;
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
    
    private async Task NotifyGroupAsync(long chatId, IEnumerable<GetTaskDto> tasks, CancellationToken cancellationToken)
    {
        var stringBuilder = GetBasicPopulatedStringBuilder();
        
        PopulateStringBuilderGroupedByNotLevelTasks(ref stringBuilder, tasks, _dataPresentationService.GetPlainTextRepresentation);

        try
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: stringBuilder.ToString(),
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);
        }
        catch (Exception)
        {
            // Ignore
        }
    }
    
    private async Task NotifyUserAsync(long userId, Dictionary<string, List<GetTaskDto>> tasks, CancellationToken cancellationToken)
    {
        try
        {
            await _botClient.SendTextMessageAsync(
                chatId: userId,
                text: GetBasicPopulatedStringBuilder().ToString(),
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);
        }
        catch (Exception)
        {
            // Ignore
        }
        
        foreach (var tuple in tasks)
        {
            var stringBuilder = new StringBuilder();
            
            var title = tuple.Key == string.Empty
                ? _localizer["word_personal"]
                : tuple.Key;
            
            stringBuilder.Append('*');
            stringBuilder.Append(_localizer["word_group"].Value.EscapeCharacters());
            stringBuilder.Append(": ".EscapeCharacters());
            stringBuilder.Append(title.EscapeCharacters());
            stringBuilder.AppendLine("*\n");
            
            PopulateStringBuilderGroupedByNotLevelTasks(ref stringBuilder,
                tuple.Value, 
                _dataPresentationService.GetTabledTextRepresentation, 
                true);
            
            try
            {
                await _botClient.SendTextMessageAsync(
                    chatId: userId,
                    text: stringBuilder.ToString(),
                    parseMode: ParseMode.MarkdownV2,
                    cancellationToken: cancellationToken);
            }
            catch (Exception)
            {
                // Ignore
            }
        }
    }

    private StringBuilder GetBasicPopulatedStringBuilder()
    {
        var stringBuilder = new StringBuilder();
        
        var reminder = _localizer["deadline_notification_reminder"].Value.EscapeCharacters();

        stringBuilder.Append('*');
        stringBuilder.Append(reminder);
        stringBuilder.AppendLine("*\n");

        return stringBuilder;
    }

    private void PopulateStringBuilderGroupedByNotLevelTasks(ref StringBuilder stringBuilder,
        IEnumerable<GetTaskDto> tasks, Func<IEnumerable<GetTaskDto>, string> dataPresentationMethod,
        bool tableView = false)
    {
        var tasksGroupedByNotificationLevel = tasks.GroupBy(x => x.NotificationLevel);

        foreach (var grouping in tasksGroupedByNotificationLevel)
        {
            var header = _localizer[NotificationLevelCustomEnum.FromValue((int)grouping.Key).DisplayName].Value
                .EscapeCharacters();
            var plainText = dataPresentationMethod(grouping).EscapeCharacters();

            stringBuilder.Append('*');
            stringBuilder.Append(header);
            stringBuilder.AppendLine("*");
            if (tableView) stringBuilder.Append("```");
            stringBuilder.AppendLine(plainText);
            if (tableView) stringBuilder.Append("```");
            stringBuilder.AppendLine();
        }
    }
}