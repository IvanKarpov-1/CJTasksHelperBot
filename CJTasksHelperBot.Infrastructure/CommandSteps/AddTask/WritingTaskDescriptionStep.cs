using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Extensions;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CJTasksHelperBot.Infrastructure.CommandSteps.AddTask;

public class WritingTaskDescriptionStep : IStep
{
	private readonly ITelegramBotClient _botClient;
	private readonly ICacheService _cacheService;

	public WritingTaskDescriptionStep(ITelegramBotClient botClient, ICacheService cacheService)
	{
		_botClient = botClient;
		_cacheService = cacheService;
	}

	public CommandStep CommandStep { get; set; } = CommandStep.WritingTaskDescription;

	public async Task PerformStepAsync(UserDto userDto, ChatDto chatDto, string text, CancellationToken cancellationToken)
	{
		_cacheService.AddValueToDictionaryOfExistingStateObject(userDto.Id, chatDto.Id, CommandStep.DisplayName, text,
			CommandStep.WritingTaskDeadline);

		var example = "(Приклад: 09.07.2023 16:45)".EscapeCharacters();

		await _botClient.SendTextMessageAsync(
			chatId: chatDto.Id,
			text: $"Введіть дедлайн завдання у форматі `dd.MM.yyyy HH:mm` або `dd.MM.yyyy` {example}" ,
			parseMode: ParseMode.MarkdownV2,
			cancellationToken: cancellationToken);
	}
}