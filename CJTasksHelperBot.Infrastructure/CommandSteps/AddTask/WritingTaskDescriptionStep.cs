using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Extensions;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using Telegram.Bot;

namespace CJTasksHelperBot.Infrastructure.CommandSteps.AddTask;

public class WritingTaskDescriptionStep : IStep
{
	private readonly ITelegramBotClient _botClient;
	private readonly ICacheService _commandStateService;

	public WritingTaskDescriptionStep(ITelegramBotClient botClient, ICacheService commandStateService)
	{
		_botClient = botClient;
		_commandStateService = commandStateService;
	}

	public CommandStep CommandStep { get; set; } = CommandStep.WritingTaskDescription;

	public async Task PerformStepAsync(UserDto userDto, ChatDto chatDto, string text, CancellationToken cancellationToken)
	{
		_commandStateService.AddValueToDictionaryOfExistingStateObject(userDto.Id, chatDto.Id, CommandStep.DisplayName, text,
			CommandStep.WritingTaskDeadline);

		await _botClient.SendTextMessageAsync(
			chatId: chatDto.Id,
			text: "Введіть дедлайн завдання у форматі \"dd.MM.yyyy HH:mm\" (Приклад: 09.07.2023 16:45)",
			cancellationToken: cancellationToken);
	}
}