using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.Task.Commands;
using CJTasksHelperBot.Infrastructure.Common;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using MediatR;
using System.Globalization;
using CJTasksHelperBot.Infrastructure.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot;

namespace CJTasksHelperBot.Infrastructure.CommandSteps.AddTask;

public class WritingTaskDeadlineStep : IStep
{
	private readonly ITelegramBotClient _botClient;
	private readonly ICacheService _cacheService;
	private readonly IMediator _mediator;
	private readonly IStringLocalizer<Messages> _localizer;

	public WritingTaskDeadlineStep(ITelegramBotClient botClient, ICacheService cacheService, IMediator mediator, IStringLocalizer<Messages> localizer)
	{
		_botClient = botClient;
		_cacheService = cacheService;
		_mediator = mediator;
		_localizer = localizer;
	}

	public CommandStep CommandStep => CommandStep.WritingTaskDeadline;

	public async Task PerformStepAsync(UserDto userDto, ChatDto chatDto, string text, CancellationToken cancellationToken)
	{
		var formats = new[] { "dd.MM.yyyy HH:mm", "dd.MM.yyyy" };
		var provider = CultureInfo.InvariantCulture;

		var result = DateTime.TryParseExact(text, formats, provider, DateTimeStyles.None, out var deadline);

		if (result == false)
		{
			await _botClient.SendTextMessageAsync(
				chatId: chatDto.Id,
				text: _localizer["wrong_datetime_try_again"],
				cancellationToken: cancellationToken);

			return;
		}

		var stateObject = _cacheService.Get<StateObject>(userDto.Id, chatDto.Id);

		_cacheService.Delete(userDto.Id, chatDto.Id);

		if (stateObject == null)
		{
			await _botClient.SendTextMessageAsync(
				chatId: chatDto.Id,
				text: _localizer["err_creat_task_try_again"],
				cancellationToken: cancellationToken);

			return;
		}

		var createTaskDto = new CreateTaskDto
		{
			Title = stateObject.Values[CommandStep.WritingTaskTitle.DisplayName].ToString(),
			Description = stateObject.Values[CommandStep.WritingTaskDescription.DisplayName].ToString(),
			Deadline = deadline,
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