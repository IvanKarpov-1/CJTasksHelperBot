using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.LanguageCode.Commands;
using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Infrastructure.Common;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Extensions;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CJTasksHelperBot.Infrastructure.Commands;

public class ChangeLanguageCommand : ICommand
{
    private readonly ITelegramBotClient _botClient;
    private readonly ICacheService _cacheService;
    private readonly IMediator _mediator;
    private readonly ICallbackQueryService _callbackQueryService;

    public ChangeLanguageCommand(ITelegramBotClient botClient, ICacheService cacheService, IMediator mediator, ICallbackQueryService callbackQueryService)
    {
        _botClient = botClient;
        _cacheService = cacheService;
        _mediator = mediator;
        _callbackQueryService = callbackQueryService;
    }

    public CommandType CommandType => CommandType.ChangeLanguage;
    public bool IsAllowCommandLineArguments => true;
    public async Task ExecuteAsync(UserDto userDto, ChatDto chatDto, CancellationToken cancellationToken)
    {
        if (userDto.Id != chatDto.Id)
        {
            await SendMessage(userDto, chatDto, cancellationToken);
        }
        else
        {
            await SendInlineKeyboard(userDto, chatDto, cancellationToken);
        }
    }

    public async Task ExecuteWithCommandLineArguments(UserDto userDto, ChatDto chatDto, Dictionary<string, string> arguments,
        CancellationToken cancellationToken)
    {
        var language = arguments.GetArgument(CommandLineArgument.Language);

        if (language == string.Empty)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatDto.Id,
                text: $"Потрібно ввести код мови `{CommandLineArgument.Language.DisplayName}`",
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);
			
            return;
        }

        try
        {
            var langId = LanguageCodeCustomEnum.FromDisplayName(language).Id;

            if (Enum.IsDefined(typeof(LanguageCode), langId))
            {
                await _mediator.Send(new ChangeChatLanguageCodeQuery { ChatId = chatDto.Id, LanguageCode = (LanguageCode)langId }, cancellationToken);
            }
        }
        catch (InvalidOperationException)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatDto.Id,
                text: $"Введено невірне значення. Спробуйте ще раз",
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);
        }
    }

    private async Task SendMessage(UserDto userDto, ChatDto chatDto, CancellationToken cancellationToken)
    {
        var stateObject = new StateObject
        {
            CallingCommand = CommandType.DisplayName,
            CurrentStep = CommandStep.WritingLanguageCode.DisplayName
        };

        _cacheService.Add(userDto.Id, chatDto.Id, stateObject);

        var availableLangCodes = LanguageCodeCustomEnum.GetAll().Select(x => x.DisplayName);

        var message = "Щоб перервати виконання команди, напишіть /stop\n" +
                          "\n" +
                          "Введіть код мови. Доступні коди: ".EscapeCharacters();

        message += '`' + string.Join("`, `", availableLangCodes) + '`';
        
        await _botClient.SendTextMessageAsync(
            chatId: chatDto.Id,
            text: message,
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);
    }

    private async Task SendInlineKeyboard(UserDto userDto, ChatDto chatDto, CancellationToken cancellationToken)
    {
        var availableLangCodes = LanguageCodeCustomEnum.GetAll().Select(x => x.DisplayName);
        
        const int maxButtonWidth = 30;
        var availableWidth = Math.Min(4096, maxButtonWidth * 4);
        var inlineKeyboard = new List<List<InlineKeyboardButton>>();
        var currentRow = new List<InlineKeyboardButton>();
        var currentRowWidth = 0;

        foreach (var langCode in availableLangCodes)
        {
            var buttonWidth = langCode.Length;
            
            if (currentRowWidth + buttonWidth > availableWidth)
            {
                inlineKeyboard.Add(currentRow);
                currentRow = [];
                currentRowWidth = 0;
            }

            var button = InlineKeyboardButton.WithCallbackData(
                text: langCode,
                callbackData: _callbackQueryService.CreateQuery<CallbackQueries.ChangeLanguageQuery>(userDto,
                    (CallbackQueriesDataKey.LangCode.DisplayName, langCode)));

            currentRow.Add(button);
            currentRowWidth += buttonWidth;
        }
        
        if (currentRow.Count > 0) inlineKeyboard.Add(currentRow);
        
        await _botClient.SendTextMessageAsync(
            chatId: chatDto.Id,
            text: "Виберіть мову",
            replyMarkup: new InlineKeyboardMarkup(inlineKeyboard),
            cancellationToken: cancellationToken);
    }
}