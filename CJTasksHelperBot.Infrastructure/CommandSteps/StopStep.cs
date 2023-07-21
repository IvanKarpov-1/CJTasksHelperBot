using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using Telegram.Bot;

namespace CJTasksHelperBot.Infrastructure.CommandSteps;

public class StopStep : IStep
{
	private readonly ITelegramBotClient _botClient;
	private readonly ICacheService _commandStateService;

	public StopStep(ITelegramBotClient botClient, ICacheService commandStateService)
	{
		_botClient = botClient;
		_commandStateService = commandStateService;
	}

	public CommandStep CommandStep { get; set; } = CommandStep.Stop;

	public async Task PerformStepAsync(UserDto userDto, ChatDto chatDto, string text, CancellationToken cancellationToken)
	{
		var stateObject = _commandStateService.Get<StateObject>(userDto.Id, chatDto.Id);
		var command = stateObject?.CallingCommand;

		_commandStateService.Delete(userDto.Id, chatDto.Id);

		await _botClient.SendTextMessageAsync(
			chatId: chatDto.Id,
			text: $"Команда {command} успішно перервана",
			cancellationToken: cancellationToken);
	}
}