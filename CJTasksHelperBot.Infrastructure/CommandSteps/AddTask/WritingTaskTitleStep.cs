﻿using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Extensions;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using Telegram.Bot;

namespace CJTasksHelperBot.Infrastructure.CommandSteps.AddTask;

public class WritingTaskTitleStep : IStep
{
	private readonly ITelegramBotClient _botClient;
	private readonly ICacheService _cacheService;

	public WritingTaskTitleStep(ITelegramBotClient botClient, ICacheService cacheService)
	{
		_botClient = botClient;
		_cacheService = cacheService;
	}

	public CommandStep CommandStep { get; set; } = CommandStep.WritingTaskTitle;

	public async Task PerformStepAsync(UserDto userDto, ChatDto chatDto, string text, CancellationToken cancellationToken)
	{
		_cacheService.AddValueToDictionaryOfExistingStateObject(userDto.Id, chatDto.Id, CommandStep.DisplayName, text, CommandStep.WritingTaskDescription);

		await _botClient.SendTextMessageAsync(
			chatId: chatDto.Id,
			text: "Введіть опис завдання",
			cancellationToken: cancellationToken);
	}
}