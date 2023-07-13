using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.Task.Commands;
using CJTasksHelperBot.Infrastructure.Common;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Extensions;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using System.Globalization;
using MediatR;
using Telegram.Bot;
using static System.Net.Mime.MediaTypeNames;

namespace CJTasksHelperBot.Infrastructure.Commands;

public class AddTaskCommand : ICommand
{
	private readonly ITelegramBotClient _botClient;
	private readonly ICacheService _commandStateService;
	private readonly IMediator _mediator;

	public AddTaskCommand(ITelegramBotClient botClient, ICacheService commandStateService, IMediator mediator)
	{
		_botClient = botClient;
		_commandStateService = commandStateService;
		_mediator = mediator;
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

		_commandStateService.Add(userDto.Id, chatDto.Id, stateObject);

		await _botClient.SendTextMessageAsync(
			chatId: chatDto.Id,
			text: "Щоб перервати виконання команди, напишіть /stop\n" +
			      "\n" +
			      "Введіть назву завдання",
			cancellationToken: cancellationToken);
	}

	public async Task ExecuteWithCommandLineArguments(UserDto userDto, ChatDto chatDto, Dictionary<string, string> arguments,
		CancellationToken cancellationToken)
	{
		var title = arguments.GetArgument(CommandLineArgument.Title);
		var description = arguments.GetArgument(CommandLineArgument.Description);
		var deadline = arguments.GetArgument(CommandLineArgument.Deadline);
		var deadlineDt = DateTime.MinValue;

		if (title == string.Empty)
		{
			await _botClient.SendTextMessageAsync(
				chatId: chatDto.Id,
				text: $"Потрібно ввести назву задачі {CommandLineArgument.Title}",
			cancellationToken: cancellationToken);
			
			return;
		}

		const string format = "dd.MM.yyyy HH:mm";
		var provider = CultureInfo.InvariantCulture;

		if (deadline != string.Empty)
		{
			var result = DateTime.TryParseExact(deadline, format, provider, DateTimeStyles.None, out deadlineDt);
			
			if (result == false)
			{
				await _botClient.SendTextMessageAsync(
					chatId: chatDto.Id,
					text: "Неправильно введені дата та час. Спробуйте ще раз",
					cancellationToken: cancellationToken);

				return;
			}
		}

		var createTaskDto = new CreateTaskDto
		{
			Title = title,
			Description = description,
			Deadline = deadlineDt,
			UserChatDto = new UserChatDto
			{
				UserId = userDto.Id,
				UserDto = userDto,
				ChatId = chatDto.Id,
				ChatDto = chatDto
			}
		};

		await _mediator.Send(new CreateTaskCommand { CreateTaskDto = createTaskDto }, cancellationToken);

		await _botClient.SendTextMessageAsync(
			chatId: chatDto.Id,
			text: "Задачу було успішно створено",
			cancellationToken: cancellationToken);
	}
}