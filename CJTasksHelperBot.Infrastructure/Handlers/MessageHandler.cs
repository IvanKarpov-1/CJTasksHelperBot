﻿using CJTasksHelperBot.Infrastructure.Common.Interfaces.Handlers;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Handlers;

public class MessageHandler : IMessageHandler
{
	private readonly ICommandService _commandService;
	private readonly IUserService _userService;
	private readonly IChatService _chatService;
	private readonly IUserChatService _userChatService;

	public MessageHandler(ICommandService commandService, IUserService userService, IChatService chatService, IUserChatService userChatService)
	{
		_commandService = commandService;
		_userService = userService;
		_chatService = chatService;
		_userChatService = userChatService;
	}

	public async Task HandleMessageAsync(Message message, CancellationToken cancellationToken)
	{
		var execute = message switch
		{
			{ Text: { } text } => HandleTextMessageAsync(message, cancellationToken),
			_ => HandleUnknownMessageType(message, cancellationToken)
		};

		await execute;
	}

	public async Task HandleTextMessageAsync(Message message, CancellationToken cancellationToken)
	{
		var user = await _userService.GetUserFromTelegramModelAsync(message.From);
		var chat = await _chatService.GetChatFromTelegramModelAsync(message.Chat);
		await _userChatService.CreateUserChatAsync(user.Id, chat.Id);

		await _commandService.InitializeAsync();

		if (_commandService.IsCommand(message.Text))
		{
			await _commandService.HandleTextCommandAsync(user, chat, message.Text, cancellationToken);
		}
		else
		{
			
		}
	}

	public Task HandleUnknownMessageType(Message message, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}