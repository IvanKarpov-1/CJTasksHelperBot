using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot;

namespace CJTasksHelperBot.Infrastructure.CommandSteps;

public class StopStep : IStep
{
	private readonly ITelegramBotClient _botClient;
	private readonly ICacheService _commandStateService;
	private readonly IStringLocalizer<Messages> _localizer;

	public StopStep(ITelegramBotClient botClient, ICacheService commandStateService, IStringLocalizer<Messages> localizer)
	{
		_botClient = botClient;
		_commandStateService = commandStateService;
		_localizer = localizer;
	}

	public CommandStep CommandStep { get; set; } = CommandStep.Stop;

	public async Task PerformStepAsync(UserDto userDto, ChatDto chatDto, string text, CancellationToken cancellationToken)
	{
		var stateObject = _commandStateService.Get<StateObject>(userDto.Id, chatDto.Id);
		var command = stateObject?.CallingCommand;

		_commandStateService.Delete(userDto.Id, chatDto.Id);

		await _botClient.SendTextMessageAsync(
			chatId: chatDto.Id,
			text: $"{_localizer["comm_succ_int"]}: {command}",
			cancellationToken: cancellationToken);
	}
}