using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.LanguageCode.Commands;
using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Resources;
using MediatR;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CJTasksHelperBot.Infrastructure.CommandSteps;

public class WritingLanguageCodeStep : IStep
{
    private readonly ITelegramBotClient _botClient;
    private readonly ICacheService _cacheService;
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<Messages> _localizer;

    public WritingLanguageCodeStep(ITelegramBotClient botClient, ICacheService cacheService, IMediator mediator, IStringLocalizer<Messages> localizer)
    {
        _botClient = botClient;
        _cacheService = cacheService;
        _mediator = mediator;
        _localizer = localizer;
    }
    
    public CommandStep CommandStep => CommandStep.WritingLanguageCode;
    
    public async Task PerformStepAsync(UserDto userDto, ChatDto chatDto, string text, CancellationToken cancellationToken)
    {
        try
        {
            var langId = LanguageCodeCustomEnum.FromDisplayName(text).Id;

            if (!Enum.IsDefined(typeof(LanguageCode), langId)) throw new ArgumentException("");
            
            await _mediator.Send(new ChangeChatLanguageCodeCommand { ChatId = chatDto.Id, LanguageCode = (LanguageCode)langId }, cancellationToken);
            
            _cacheService.Delete(userDto.Id, chatDto.Id);
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatDto.Id,
                text: _localizer["wrng_val_enter_again"],
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);
        }
    }
}