﻿using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.Task.Commands;
using CJTasksHelperBot.Infrastructure.Common;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using MediatR;
using System.Globalization;
using Telegram.Bot;

namespace CJTasksHelperBot.Infrastructure.CommandSteps.AddTask;

public class WritingTaskDeadlineStep : IStep
{
	private readonly ITelegramBotClient _botClient;
	private readonly ICommandStateService _commandStateService;
	private readonly IMediator _mediator;

	public WritingTaskDeadlineStep(ITelegramBotClient botClient, ICommandStateService commandStateService, IMediator mediator)
	{
		_botClient = botClient;
		_commandStateService = commandStateService;
		_mediator = mediator;
	}

	public CommandStep CommandStep { get; set; } = CommandStep.WritingTaskDeadline;

	public async Task PerformStepAsync(UserDto userDto, ChatDto chatDto, string text, CancellationToken cancellationToken)
	{
		const string format = "dd.MM.yyyy HH:mm";
		var provider = CultureInfo.InvariantCulture;

		var result = DateTime.TryParseExact(text, format, provider, DateTimeStyles.None, out var deadline);

		if (result == false)
		{
			await _botClient.SendTextMessageAsync(
				chatId: chatDto.Id,
				text: "Неправильно введені дата та час. Спробуйте ще раз",
				cancellationToken: cancellationToken);

			return;
		}

		var stateObject = _commandStateService.GetStateObject<StateObject>(userDto.Id, chatDto.Id);

		_commandStateService.DeleteStateObject<StateObject>(userDto.Id, chatDto.Id);

		if (stateObject == null)
		{
			await _botClient.SendTextMessageAsync(
				chatId: chatDto.Id,
				text: "Сталая помилка під час створення завдання. Спробуйте ще раз заново",
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
			text: "Задачу було успішно створено",
			cancellationToken: cancellationToken);
	}
}