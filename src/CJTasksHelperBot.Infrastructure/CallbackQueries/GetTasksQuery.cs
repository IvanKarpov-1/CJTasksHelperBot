using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Extensions;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Common.Mapping;
using MediatR;
using Newtonsoft.Json;
using CJTasksHelperBot.Infrastructure.Resources;
using CJTasksHelperBot.Infrastructure.Services;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Infrastructure.CallbackQueries;

public class GetTasksQuery : ICallbackQuery
{
	private CallbackQuery? _callbackQuery;
	private readonly ITelegramBotClient _botClient;
	private readonly ICacheService _cacheService;
	private readonly IMediator _mediator;
	private readonly IDataPresentationService _dataPresentationService;
	private readonly IStringLocalizer<Messages> _localizer;
	private readonly MapperlyMapper _mapper;
	private readonly CallbackQueryService _callbackQueryService;

	public GetTasksQuery(ITelegramBotClient botClient, ICacheService cacheService, IMediator mediator, IDataPresentationService dataPresentationService, IStringLocalizer<Messages> localizer, MapperlyMapper mapper)
	{
		_botClient = botClient;
		_cacheService = cacheService;
		_mediator = mediator;
		_dataPresentationService = dataPresentationService;
		_localizer = localizer;
		_mapper = mapper;
		_callbackQueryService = new CallbackQueryService(
			[],
			mapper,
			cacheService,
			new LocalizationService(mediator));
	}

	public bool IsRequireCallbackQuery => true;

	public void SetCallbackQuery(CallbackQuery callbackQuery)
	{
		_callbackQuery = callbackQuery;
	}

	public async Task ExecuteAsync(UserDto userDto, ChatDto chatDto, Dictionary<string, object> data, CancellationToken cancellationToken)
	{
		if (data.TryGetValue(CallbackQueriesDataKey.Toggle.DisplayName, out var toggleObj))
		{
			var toggle = toggleObj.ToString();

			if (string.IsNullOrEmpty(toggle) == false)
			{
				await ChangeNeedOfDrawingTable(data, cancellationToken);
				return;
			}
		}

		if (data.TryGetValue(CallbackQueriesDataKey.StatusSwitch.DisplayName, out var statusSwitchObj))
		{
			var statusSwitch = statusSwitchObj.ToString();

			if (statusSwitch == "")
			{
				await ShowStatusesButtons(cancellationToken);
				return;
			}
		}

		if (data.TryGetValue(CallbackQueriesDataKey.StatusSelected.DisplayName, out var statusSelectedObj))
		{
			var statusSelected = statusSelectedObj.ToString();

			if (statusSelected != null)
			{
				await ChangeStatus(data, cancellationToken);
				return;
			}
		}

		if (data.ContainsKey(CallbackQueriesDataKey.TelegramId.DisplayName) == false) return;
		if (data.ContainsKey(CallbackQueriesDataKey.IsNeedDrawTable.DisplayName) == false) return;
		if (data.ContainsKey(CallbackQueriesDataKey.Status.DisplayName) == false) return;

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
					button.Text = $"{_localizer["in_table_view"]} {flag}";
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

	private async Task ShowStatusesButtons(CancellationToken cancellationToken)
	{
		var oldReplyMarkup = _callbackQuery!.Message!.ReplyMarkup;
		
		var serializedObject = JsonConvert.SerializeObject(oldReplyMarkup);
		_cacheService.Add($"{_callbackQuery.Message.Chat.Id}{CallbackQueriesDataKey.StatusSelected.DisplayName}", serializedObject);

		var userDto = _mapper.Map(_callbackQuery.Message.From!);
		
		var replyMarkup = new InlineKeyboardMarkup(TaskStatusCustomEnum.GetAll().Select(status =>
			new List<InlineKeyboardButton>
			{
				InlineKeyboardButton.WithCallbackData(
					text: _localizer[status.DisplayName],
					callbackData: _callbackQueryService.CreateQuery<GetTasksQuery>(userDto,
						(CallbackQueriesDataKey.StatusSelected.DisplayName, status.DisplayName)))
			})
			.Append(
			[
				InlineKeyboardButton.WithCallbackData(
					text: _localizer["word_all"],
					callbackData: _callbackQueryService.CreateQuery<GetTasksQuery>(userDto,
						(CallbackQueriesDataKey.StatusSelected.DisplayName, "")))
			]));

		await _botClient.EditMessageTextAsync(
			text: _localizer["choose_status"], 
			chatId: _callbackQuery.Message.Chat.Id,
			messageId: _callbackQuery.Message.MessageId,
			replyMarkup: replyMarkup,
			cancellationToken: cancellationToken);
	}

	private async Task ChangeStatus(IReadOnlyDictionary<string, object> data, CancellationToken cancellationToken)
	{
		var serializedObject =
			_cacheService.Get<string>(
				$"{_callbackQuery!.Message!.Chat.Id}{CallbackQueriesDataKey.StatusSelected.DisplayName}");

		if (serializedObject == null)
			return;
		
		var replyMarkup = JsonConvert.DeserializeObject<InlineKeyboardMarkup>(serializedObject);
		
		var buttons = replyMarkup!.InlineKeyboard;

		foreach (var buttonRow in buttons)
		{
			foreach (var button in buttonRow)
			{
				var key = button.CallbackData;
				var dataExist = key != null && _cacheService.CheckExisting(key);

				if (!dataExist) continue;

				serializedObject = _cacheService.Get<string>(key!);
				if (serializedObject == null) continue;

				var deserializeObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedObject);
				if (deserializeObject == null) continue;

				if (deserializeObject.ContainsKey(CallbackQueriesDataKey.Status.DisplayName))
				{
					deserializeObject[CallbackQueriesDataKey.Status.DisplayName] =
						data[CallbackQueriesDataKey.StatusSelected.DisplayName].ToString()!;
				}

				if (deserializeObject.ContainsKey(CallbackQueriesDataKey.StatusSwitch.DisplayName))
				{
					var statusName = data[CallbackQueriesDataKey.StatusSelected.DisplayName].ToString()!;
					var status = _localizer["word_all"];
					if (statusName != "")
						status = _localizer[statusName];
					button.Text = $"{_localizer["word_status"]}: {status}";
				}

				serializedObject = JsonConvert.SerializeObject(deserializeObject);
				_cacheService.Update(key!, serializedObject);
			}
		}
		
		await _botClient.EditMessageTextAsync(
			text: _localizer["choose_chat_or_params"],
			chatId: _callbackQuery.Message.Chat.Id,
			messageId: _callbackQuery.Message.MessageId,
			replyMarkup: replyMarkup,
			cancellationToken: cancellationToken);
	}
	
	private async Task<IEnumerable<UserTaskStatusDto>?> GetUserTaskStatuses(IReadOnlyDictionary<string, object> data, CancellationToken cancellationToken)
	{
		var id = long.Parse(data[CallbackQueriesDataKey.TelegramId.DisplayName].ToString()!);
		
		TaskStatus? taskStatus = null;
		var statusStr = data[CallbackQueriesDataKey.Status.DisplayName].ToString();

		if (string.IsNullOrEmpty(statusStr) == false)
			taskStatus = (TaskStatus)TaskStatusCustomEnum.FromDisplayName(statusStr).Id;
		
		var result = await _mediator.Send(new Application.UserTaskStatus.Queries.GetUserTaskStatusesFromChatForUserQuery { UserId = _callbackQuery!.From.Id, ChatId = id, Status = taskStatus}, cancellationToken);
		var userTaskStatuses = result.Value?.OrderBy(x => x.Task!.Deadline);
		return userTaskStatuses;
	}

	private async Task ShowInTable(IReadOnlyDictionary<string, object> data, CancellationToken cancellationToken)
	{
		var tasks = (await GetUserTaskStatuses(data, cancellationToken) ?? Array.Empty<UserTaskStatusDto>()).ToList();

		var table = tasks.Count != 0 
			? "```" + _dataPresentationService.GetTabledTextRepresentation(tasks).EscapeCharacters() + "```"
			: _localizer["no_tasks"];

		var oldText = "```" + _callbackQuery!.Message!.Text!.EscapeCharacters() + "\n```";
		
		if (oldText == table || _callbackQuery!.Message!.Text == _localizer["no_tasks"]) return;
		
		await _botClient.EditMessageTextAsync(
			text: table,
			chatId: _callbackQuery!.Message!.Chat.Id,
			messageId: _callbackQuery.Message.MessageId,
			parseMode: ParseMode.MarkdownV2,
			replyMarkup: _callbackQuery.Message.ReplyMarkup,
			cancellationToken: cancellationToken);
	}

	private async Task ShowNotInTable(IReadOnlyDictionary<string, object> data, CancellationToken cancellationToken)
	{
		var tasks = (await GetUserTaskStatuses(data, cancellationToken) ?? Array.Empty<UserTaskStatusDto>()).ToList();

		var plainText = _dataPresentationService.GetPlainTextRepresentation(tasks);

		if (_callbackQuery!.Message!.Text == plainText|| _callbackQuery!.Message!.Text == _localizer["no_tasks"])
			return;

		await _botClient.EditMessageTextAsync(
			text: plainText,
			chatId: _callbackQuery!.Message!.Chat.Id,
			messageId: _callbackQuery.Message.MessageId,
			replyMarkup: _callbackQuery.Message.ReplyMarkup,
			cancellationToken: cancellationToken);
	}
}