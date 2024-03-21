using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.LanguageCode.Commands;
using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Infrastructure.Common;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Extensions;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Resources;
using MediatR;
using Microsoft.Extensions.Localization;
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
    private readonly IStringLocalizer<Messages> _localizer;

    public ChangeLanguageCommand(ITelegramBotClient botClient, ICacheService cacheService, IMediator mediator,
        ICallbackQueryService callbackQueryService, IStringLocalizer<Messages> localizer)
    {
        _botClient = botClient;
        _cacheService = cacheService;
        _mediator = mediator;
        _callbackQueryService = callbackQueryService;
        _localizer = localizer;
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

    public async Task ExecuteWithCommandLineArguments(UserDto userDto, ChatDto chatDto,
        Dictionary<string, string> arguments,
        CancellationToken cancellationToken)
    {
        var language = arguments.GetArgument(CommandLineArgument.Language);

        if (language == string.Empty)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatDto.Id,
                text: $"{_localizer["need_enter_lang_code"]} `{CommandLineArgument.Language.DisplayName}`",
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);

            return;
        }

        try
        {
            var langId = LanguageCodeCustomEnum.FromDisplayName(language).Id;

            if (Enum.IsDefined(typeof(LanguageCode), langId))
            {
                await _mediator.Send(
                    new ChangeChatLanguageCodeQuery { ChatId = chatDto.Id, LanguageCode = (LanguageCode)langId },
                    cancellationToken);
            }
        }
        catch (InvalidOperationException)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatDto.Id,
                text: _localizer["wrng_val_enter_again"],
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);
        }
    }

    private async Task SendMessage(UserDto userDto, ChatDto chatDto, CancellationToken cancellationToken)
    {
        var admins = await _botClient.GetChatAdministratorsAsync(chatDto.Id, cancellationToken);

        if (admins.Any(x => x.User.Id == userDto.Id) == false)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatDto.Id,
                text: _localizer["only_for_admins"],
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);
            return;
        }

        var stateObject = new StateObject
        {
            CallingCommand = CommandType.DisplayName,
            CurrentStep = CommandStep.WritingLanguageCode.DisplayName
        };

        _cacheService.Add(userDto.Id, chatDto.Id, stateObject);

        var availableLangCodes = LanguageCodeCustomEnum.GetAll().Select(x => x.DisplayName);

        var message = $"{_localizer["to_int_enter"]}\n\n{_localizer["enter_lang_code"]}".EscapeCharacters();

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
            text: _localizer["choose_lang"],
            replyMarkup: new InlineKeyboardMarkup(inlineKeyboard),
            cancellationToken: cancellationToken);
    }
}