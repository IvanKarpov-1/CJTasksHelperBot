using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Handlers;

public class MessageHandler : IMessageHandler
{
	private readonly ICommandService _commandService;

	public MessageHandler(ICommandService commandService)
	{
		_commandService = commandService;
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
		//_commandService.Initialize();

		if (_commandService.IsCommand(message.Text))
		{
			await _commandService.HandleTextCommandAsync(message, message.Text, cancellationToken);
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