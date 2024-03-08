using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.LanguageCode.Commands;
using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.CallbackQueries;

public class ChangeLanguageQuery : ICallbackQuery
{
    private CallbackQuery? _callbackQuery;
    private readonly ITelegramBotClient _botClient;
    private readonly ICacheService _cacheService;
    private readonly IMediator _mediator;

    public ChangeLanguageQuery(ITelegramBotClient botClient, ICacheService cacheService, IMediator mediator)
    {
        _botClient = botClient;
        _cacheService = cacheService;
        _mediator = mediator;
    }

    public bool IsRequireCallbackQuery => true;
    
    public void SetCallbackQuery(CallbackQuery callbackQuery)
    {
        _callbackQuery = callbackQuery;
    }

    public async Task ExecuteAsync(UserDto userDto, ChatDto chatDto, Dictionary<string, object> data, CancellationToken cancellationToken)
    {
        if (data.ContainsKey(CallbackQueriesDataKey.LangCode.DisplayName) == false) return;
            
        var langCode = data[CallbackQueriesDataKey.LangCode.DisplayName].ToString();

        if (langCode != null)
        {
            var langId = LanguageCodeCustomEnum.FromDisplayName(langCode).Id;
        
            await _mediator.Send(new ChangeChatLanguageCodeQuery { ChatId = chatDto.Id, LanguageCode = (LanguageCode)langId }, cancellationToken);
        }

        if (_callbackQuery is { Message: not null })
        {
            var messageId = _callbackQuery.Message.MessageId;

            await _botClient.DeleteMessageAsync(chatDto.Id, messageId, cancellationToken);
        }

        _cacheService.Delete(userDto.Id, chatDto.Id);
    }
}