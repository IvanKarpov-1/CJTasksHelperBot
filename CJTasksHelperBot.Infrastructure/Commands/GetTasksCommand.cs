using CJTasksHelperBot.Application.Chat.Queries;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.Task.Queries;
using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Extensions;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using MediatR;
using System.Text;
using CJTasksHelperBot.Infrastructure.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CJTasksHelperBot.Infrastructure.Commands;

public class GetTasksCommand : ICommand
{
	private readonly ITelegramBotClient _botClient;
	private readonly IMediator _mediator;
	private readonly IDataPresentationService _dataPresentationService;
	private readonly ICallbackQueryService _callbackQueryService;
	private readonly IStringLocalizer<Messages> _localizer;
	private bool _isNeedDrawTable;

	public GetTasksCommand(ITelegramBotClient botClient, IMediator mediator, IDataPresentationService dataPresentationService, ICallbackQueryService callbackQueryService, IStringLocalizer<Messages> localizer)
	{
		_botClient = botClient;
		_mediator = mediator;
		_dataPresentationService = dataPresentationService;
		_callbackQueryService = callbackQueryService;
		_localizer = localizer;
	}

	public CommandType CommandType => CommandType.GetTasks;
	public bool IsAllowCommandLineArguments => true;

	public async Task ExecuteAsync(UserDto userDto, ChatDto chatDto, CancellationToken cancellationToken)
	{
		await _botClient.SendChatActionAsync(
			chatId: chatDto.Id,
			chatAction: ChatAction.Typing,
			cancellationToken: cancellationToken);

		if (userDto.Id != chatDto.Id)
		{
			await ShowNotInTable(userDto.Id, chatDto.Id, cancellationToken);
		}
		else
		{
			await SendInlineKeyboard(userDto, chatDto, cancellationToken);
		}
	}

	public async Task ExecuteWithCommandLineArguments(UserDto userDto, ChatDto chatDto, Dictionary<string, string> arguments,
		CancellationToken cancellationToken)
	{
		_isNeedDrawTable = arguments.ContainsKey(CommandLineArgument.DrawTable);

		if (userDto.Id != chatDto.Id)
		{
			if (!_isNeedDrawTable)
			{
				var tasks = await GetTasks(userDto.Id, chatDto.Id, cancellationToken);

				var table = tasks != null 
					? "```" + _dataPresentationService.GetTabledTextRepresentation(tasks).EscapeCharacters() + "```" 
					: _localizer["no_tasks"];

				await _botClient.SendTextMessageAsync(
					chatId: chatDto.Id,
					text: table,
					parseMode: ParseMode.MarkdownV2,
					cancellationToken: cancellationToken);
			}
			else
			{
				await ShowNotInTable(userDto.Id, chatDto.Id, cancellationToken);
			}
		}
		else
		{
			await SendInlineKeyboard(userDto, chatDto, cancellationToken);
		}
	}

	private async Task ShowNotInTable(long userId, long chatId, CancellationToken cancellationToken)
	{
		var tasks = (await GetTasks(userId, chatId, cancellationToken) ?? Array.Empty<GetTaskDto>()).ToList();

		var plainText = _dataPresentationService.GetPlainTextRepresentation(tasks);

		await _botClient.SendTextMessageAsync(
			chatId: chatId,
			text: plainText,
			cancellationToken: cancellationToken);
	}

	private async Task<IEnumerable<GetTaskDto>?> GetTasks(long userId, long chatId, CancellationToken cancellationToken)
	{
		var result = await _mediator.Send(new GetTasksQuery { UserId = userId, ChatId = chatId }, cancellationToken);
		var tasks = result.Value?.OrderBy(x => x.Deadline);
		return tasks;
	}

	private async Task SendInlineKeyboard(UserDto userDto, ChatDto chatDto, CancellationToken cancellationToken)
	{
		var chats = await _mediator.Send(new GetChatsWithTasksQuery { UserId = userDto.Id }, cancellationToken);

		var inlineKeyboard = chats.Value == null
			? new InlineKeyboardMarkup(ConstructGeneralButtons(userDto))
			: ConstructInlineKeyboard(chats.Value, userDto);

		await _botClient.SendTextMessageAsync(
			chatId: chatDto.Id,
			text: "Test",
			replyMarkup: inlineKeyboard,
			cancellationToken: cancellationToken);
	}

	private InlineKeyboardMarkup ConstructInlineKeyboard(List<ChatDto> items, UserDto userDto)
	{
		const int maxButtonWidth = 30;
		var availableWidth = Math.Min(4096, maxButtonWidth * 4);
		var inlineKeyboard = new List<List<InlineKeyboardButton>>();
		var currentRowWidth = 0;

		var currentRow = ConstructGeneralButtons(userDto);

		inlineKeyboard.Add(currentRow);
		currentRow = new List<InlineKeyboardButton>();

		foreach (var item in items)
		{
			var buttonWidth = item.Title!.Length;

			if (currentRowWidth + buttonWidth > availableWidth)
			{
				inlineKeyboard.Add(currentRow);
				currentRow = new List<InlineKeyboardButton>();
				currentRowWidth = 0;
			}

			var button = InlineKeyboardButton.WithCallbackData(
				text: item.Title!,
				callbackData: _callbackQueryService.CreateQuery<CallbackQueries.GetTasksQuery>(userDto,
					(CallbackQueriesDataKey.TelegramId.DisplayName, item.Id),
					(CallbackQueriesDataKey.IsNeedDrawTable.DisplayName, _isNeedDrawTable)));

			currentRow.Add(button);
			currentRowWidth += buttonWidth;
		}
		
		if (currentRow.Count > 0) inlineKeyboard.Add(currentRow);

		return new InlineKeyboardMarkup(inlineKeyboard);
	}

	private List<InlineKeyboardButton> ConstructGeneralButtons(UserDto userDto)
	{
		var flag = _isNeedDrawTable ? StringConstant.ToggleOn.DisplayName : StringConstant.ToggleOff.DisplayName;

		return
		[
			InlineKeyboardButton.WithCallbackData(
				text: _localizer["word_personal"],
				_callbackQueryService.CreateQuery<CallbackQueries.GetTasksQuery>(userDto,
					(CallbackQueriesDataKey.TelegramId.DisplayName, userDto.Id),
					(CallbackQueriesDataKey.IsNeedDrawTable.DisplayName, _isNeedDrawTable))),

			InlineKeyboardButton.WithCallbackData(
				text: $"{_localizer["in_table_view"]} {flag}",
				callbackData: _callbackQueryService.CreateQuery<CallbackQueries.GetTasksQuery>(userDto,
					(CallbackQueriesDataKey.Toggle.DisplayName, _isNeedDrawTable)))
		];
	}
}