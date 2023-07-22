using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Extensions;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using MediatR;
using Newtonsoft.Json;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CJTasksHelperBot.Infrastructure.CallbackQueries;

public class GetTasksQuery : ICallbackQuery
{
	private CallbackQuery? _callbackQuery;
	private readonly ITelegramBotClient _botClient;
	private readonly ICacheService _cacheService;
	private readonly IMediator _mediator;
	private readonly ITableService _tableService;

	public GetTasksQuery(ITelegramBotClient botClient, ICacheService cacheService, IMediator mediator, ITableService tableService)
	{
		_botClient = botClient;
		_cacheService = cacheService;
		_mediator = mediator;
		_tableService = tableService;
	}

	public bool IsRequireCallbackQuery => true;

	public void SetCallbackQuery(CallbackQuery callbackQuery)
	{
		_callbackQuery = callbackQuery;
	}

	public async Task ExecuteAsync(UserDto userDto, ChatDto chatDto, Dictionary<string, object> data, CancellationToken cancellationToken)
	{
		if (data.ContainsKey(CallbackQueriesDataKey.Toggle.DisplayName))
		{
			var toggle = data[CallbackQueriesDataKey.Toggle.DisplayName].ToString();

			if (!string.IsNullOrEmpty(toggle))
			{
				await ChangeNeedOfDrawingTable(data, cancellationToken);
				return;
			}
		}

		if (data.ContainsKey(CallbackQueriesDataKey.TelegramId.DisplayName) == false) return;
		if (data.ContainsKey(CallbackQueriesDataKey.IsNeedDrawTable.DisplayName) == false) return;

		if ((bool)data[CallbackQueriesDataKey.IsNeedDrawTable.DisplayName])
		{
			await ShowInTable(data, cancellationToken);
		}
		else
		{
			await ShowNotInTable(data, cancellationToken);
		}
	}

	private async Task ChangeNeedOfDrawingTable(IReadOnlyDictionary<string, object> data, CancellationToken cancellationToken)
	{
		var replyMarkup = _callbackQuery!.Message!.ReplyMarkup;
		var buttons = replyMarkup!.InlineKeyboard;

		foreach (var buttonRow in buttons)
		{
			foreach (var button in buttonRow)
			{
				var key = button.CallbackData;
				var dataExist = key != null && _cacheService.CheckExisting(key);

				if (!dataExist) continue;

				var serializedObject = _cacheService.Get<string>(key!);
				if (serializedObject == null) continue;

				var deserializeObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedObject);
				if (deserializeObject == null) continue;

				if (deserializeObject.ContainsKey(CallbackQueriesDataKey.IsNeedDrawTable.DisplayName))
				{
					deserializeObject[CallbackQueriesDataKey.IsNeedDrawTable.DisplayName] =
						!(bool)data[CallbackQueriesDataKey.Toggle.DisplayName];
				}

				if (deserializeObject.ContainsKey(CallbackQueriesDataKey.Toggle.DisplayName))
				{
					var toggle = !(bool)data[CallbackQueriesDataKey.Toggle.DisplayName];
					deserializeObject[CallbackQueriesDataKey.Toggle.DisplayName] = toggle;
					var flag = toggle ? StringConstant.ToggleOn.DisplayName : StringConstant.ToggleOff.DisplayName;
					button.Text = $"{StringConstant.InTableView.DisplayName} {flag}";
				}

				serializedObject = JsonConvert.SerializeObject(deserializeObject);
				_cacheService.Update(key!, serializedObject);
			}
		}

		await _botClient.EditMessageReplyMarkupAsync(
			chatId: _callbackQuery.Message.Chat.Id,
			messageId: _callbackQuery.Message.MessageId,
			replyMarkup: replyMarkup,
			cancellationToken: cancellationToken);
	}

	private async Task<IEnumerable<GetTaskDto>?> GetTasks(long id, CancellationToken cancellationToken)
	{
		var result = await _mediator.Send(new Application.Task.Queries.GetTasksQuery { ChatId = id }, cancellationToken);
		var tasks = result.Value?.OrderBy(x => x.Deadline);
		return tasks;
	}

	private async Task ShowInTable(IReadOnlyDictionary<string, object> data, CancellationToken cancellationToken)
	{
		var id = long.Parse(data[CallbackQueriesDataKey.TelegramId.DisplayName].ToString()!);
		var tasks = await GetTasks(id, cancellationToken);

		var table = tasks != null ? "`" + _tableService.GetTable(tasks).EscapeCharacters() + "`" : "Немає завдань";

		await _botClient.EditMessageTextAsync(
			chatId: _callbackQuery!.Message!.Chat.Id,
			messageId: _callbackQuery.Message.MessageId,
			text: table,
			parseMode: ParseMode.MarkdownV2,
			replyMarkup: _callbackQuery.Message.ReplyMarkup,
			cancellationToken: cancellationToken);
	}

	private async Task ShowNotInTable(IReadOnlyDictionary<string, object> data, CancellationToken cancellationToken)
	{
		var id = long.Parse(data[CallbackQueriesDataKey.TelegramId.DisplayName].ToString()!);
		var tasks = await GetTasks(id, cancellationToken);

		var tasksInfo = new StringBuilder();
		var i = 1;

		if (tasks != null)
		{
			foreach (var task in tasks)
			{
				tasksInfo.AppendLine(
					$"{i++}) {task.Title} | До: {task.Deadline} | {task.Description} | Статус: {TaskStatusCustomEnum.FromValue((int)task.Status).DisplayName};\n");
			}
		}
		else
		{
			tasksInfo.AppendLine("Немає завдань");
		}

		await _botClient.EditMessageTextAsync(
			chatId: _callbackQuery!.Message!.Chat.Id,
			messageId: _callbackQuery.Message.MessageId,
			text: tasksInfo.ToString(),
			replyMarkup: _callbackQuery.Message.ReplyMarkup,
			cancellationToken: cancellationToken);
	}
}