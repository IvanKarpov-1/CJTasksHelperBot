using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Extensions;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CJTasksHelperBot.Infrastructure.CommandSteps.UpdateTaskStatus;

public class WritingPartialTaskTitleStep : IStep
{
    private readonly ITelegramBotClient _botClient;
    private readonly ICacheService _cacheService;
    private readonly IStringLocalizer<Messages> _localizer;

    public WritingPartialTaskTitleStep(ITelegramBotClient botClient, ICacheService cacheService, IStringLocalizer<Messages> localizer)
    {
        _botClient = botClient;
        _cacheService = cacheService;
        _localizer = localizer;
    }

    public CommandStep CommandStep => CommandStep.WritingPartialTaskTitle;
    
    public async Task PerformStepAsync(UserDto userDto, ChatDto chatDto, string text, CancellationToken cancellationToken)
    {
        _cacheService.AddValueToDictionaryOfExistingStateObject(userDto.Id, chatDto.Id, CommandStep.DisplayName, text,
            CommandStep.WritingNewTaskStatus);

        var statuses =
            $"`{TaskStatusCustomEnum.NotStarted.Id}` \\- {_localizer[TaskStatusCustomEnum.NotStarted.DisplayName]}\n" +
            $"`{TaskStatusCustomEnum.InProgress.Id}` \\- {_localizer[TaskStatusCustomEnum.InProgress.DisplayName]}\n" +
            $"`{TaskStatusCustomEnum.AlmostDone.Id}` \\- {_localizer[TaskStatusCustomEnum.AlmostDone.DisplayName]}\n" +
            $"`{TaskStatusCustomEnum.Completed.Id}` \\- {_localizer[TaskStatusCustomEnum.Completed.DisplayName]}";
        
        await _botClient.SendTextMessageAsync(
            chatId: chatDto.Id,
            text: $"{_localizer["enter_new_task_status"]}:\n{statuses}",
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);
    }
}