using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.Task.Commands;
using CJTasksHelperBot.Infrastructure.Common;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Extensions;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using MediatR;
using System.Globalization;
using CJTasksHelperBot.Infrastructure.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CJTasksHelperBot.Infrastructure.Commands;

public class AddTaskCommand : ICommand
{
	private readonly ITelegramBotClient _botClient;
	private readonly ICacheService _cacheService;
	private readonly IMediator _mediator;
	private readonly IStringLocalizer<Messages> _localizer;

	public AddTaskCommand(ITelegramBotClient botClient, ICacheService cacheService, IMediator mediator, IStringLocalizer<Messages> localizer)
	{
		_botClient = botClient;
		_cacheService = cacheService;
		_mediator = mediator;
		_localizer = localizer;
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

		_cacheService.Add(userDto.Id, chatDto.Id, stateObject);

		await _botClient.SendTextMessageAsync(
			chatId: chatDto.Id,
			text: $"{_localizer["to_int_enter"]}\n\n{_localizer["enter_task_name"]}",
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
				text: $"{_localizer["need_enter_task_name"]} `{CommandLineArgument.Title.DisplayName}`",
				parseMode: ParseMode.MarkdownV2,
				cancellationToken: cancellationToken);
			
			return;
		}

		var formats = new[] { "dd.MM.yyyy HH:mm", "dd.MM.yyyy" };
		var provider = CultureInfo.InvariantCulture;

		if (deadline != string.Empty)
		{
			var result = DateTime.TryParseExact(deadline, formats, provider, DateTimeStyles.None, out deadlineDt);
			
			if (result == false)
			{
				await _botClient.SendTextMessageAsync(
					chatId: chatDto.Id,
					text: _localizer["wrong_datetime_try_again"],
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
			text: _localizer["task_created"],
			cancellationToken: cancellationToken);
	}
}