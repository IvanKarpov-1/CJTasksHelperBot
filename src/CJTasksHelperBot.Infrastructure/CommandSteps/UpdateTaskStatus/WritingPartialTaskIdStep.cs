using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Extensions;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CJTasksHelperBot.Infrastructure.CommandSteps.UpdateTaskStatus;

public class WritingPartialTaskIdStep : IStep
{
    private readonly ITelegramBotClient _botClient;
    private readonly ICacheService _cacheService;
    private readonly IStringLocalizer<Messages> _localizer;

    public WritingPartialTaskIdStep(ITelegramBotClient botClient, ICacheService cacheService, IStringLocalizer<Messages> localizer)
    {
        _botClient = botClient;
        _cacheService = cacheService;
        _localizer = localizer;
    }

    public CommandStep CommandStep => CommandStep.WritingPartialTaskId;
    
    public async Task PerformStepAsync(UserDto userDto, ChatDto chatDto, string text, CancellationToken cancellationToken)
    {
        _cacheService.AddValueToDictionaryOfExistingStateObject(userDto.Id, chatDto.Id, CommandStep.DisplayName, text,
            CommandStep.WritingPartialTaskTitle);

        await _botClient.SendTextMessageAsync(
            chatId: chatDto.Id,
            text: _localizer["enter_partial_task_title"],
            cancellationToken: cancellationToken);
    }
}