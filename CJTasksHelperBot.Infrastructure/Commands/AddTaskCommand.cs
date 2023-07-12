using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using Telegram.Bot;

namespace CJTasksHelperBot.Infrastructure.Commands;

public class AddTaskCommand : ICommand
{
	private readonly ITelegramBotClient _botClient;
	private readonly ICommandStateService _commandStateService;

	public AddTaskCommand(ITelegramBotClient botClient, ICommandStateService commandStateService)
	{
		_botClient = botClient;
		_commandStateService = commandStateService;
	}

	public CommandType CommandType => CommandType.AddTask;
	public bool IsAllowCommandLineArguments => true;

	public async Task ExecuteAsync(UserDto userDto, ChatDto chatDto, CancellationToken cancellationToken)
	{
		var stateObject = new StateObject
		{
			CallingCommand = CommandType.DisplayName,
			CurrentStep = CommandStep.WritingTaskTitle.DisplayName
		};

		_commandStateService.AddStateObject(userDto.Id, chatDto.Id, stateObject);

		await _botClient.SendTextMessageAsync(
			chatId: chatDto.Id,
			text: "Щоб перервати виконання команди, напишіть /stop\n" +
			      "\n" +
			      "Введіть назву завдання",
			cancellationToken: cancellationToken);
	}

	public Task ExecuteWithCommandLineArguments(UserDto userDto, ChatDto chatDto, Dictionary<string, string> arguments,
		CancellationToken cancellationToken)
	{
		
	}
}