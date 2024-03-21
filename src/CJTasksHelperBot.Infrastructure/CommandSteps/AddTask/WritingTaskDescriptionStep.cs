using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Extensions;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CJTasksHelperBot.Infrastructure.CommandSteps.AddTask;

public class WritingTaskDescriptionStep : IStep
{
	private readonly ITelegramBotClient _botClient;
	private readonly ICacheService _cacheService;
	private readonly IStringLocalizer<Messages> _localizer;

	public WritingTaskDescriptionStep(ITelegramBotClient botClient, ICacheService cacheService, IStringLocalizer<Messages> localizer)
	{
		_botClient = botClient;
		_cacheService = cacheService;
		_localizer = localizer;
	}

	public CommandStep CommandStep => CommandStep.WritingTaskDescription;

	public async Task PerformStepAsync(UserDto userDto, ChatDto chatDto, string text, CancellationToken cancellationToken)
	{
		_cacheService.AddValueToDictionaryOfExistingStateObject(userDto.Id, chatDto.Id, CommandStep.DisplayName, text,
			CommandStep.WritingTaskDeadline);

		var exampleTime = DateTime.Now.AddDays(1).AddHours(4).ToString("dd.MM.yyyy HH:mm");

		var example = $"({_localizer["word_example"]}: {exampleTime})".EscapeCharacters();

		await _botClient.SendTextMessageAsync(
			chatId: chatDto.Id,
			text: $"{_localizer["enter_task_deadline"]} {example}" ,
			parseMode: ParseMode.MarkdownV2,
			cancellationToken: cancellationToken);
	}
}