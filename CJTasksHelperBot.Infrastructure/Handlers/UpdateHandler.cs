using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Handlers;

public class UpdateHandler : IUpdateHandler
{
	private readonly ILogger<UpdateHandler> _logger;
	private readonly IMessageHandler _messageHandler;
	//private readonly ICallbackQueryHandler _callbackQueryHandler;
	//private readonly IInlineQueryHandler _inlineQueryHandler;
	//private readonly IChosenInlineResultHandler _chosenInlineResultHandler;

	public UpdateHandler(ILogger<UpdateHandler> logger, IMessageHandler messageHandler/*, ICallbackQueryHandler callbackQueryHandler, IInlineQueryHandler inlineQueryHandler, IChosenInlineResultHandler chosenInlineResultHandler*/)
	{
		_logger = logger;
		_messageHandler = messageHandler;
		//_callbackQueryHandler = callbackQueryHandler;
		//_inlineQueryHandler = inlineQueryHandler;
		//_chosenInlineResultHandler = chosenInlineResultHandler;
	}

	public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
	{
		if (update != null)
		{
			var handler = update switch
			{
				{ Message: { } message } => _messageHandler.HandleMessageAsync(message, cancellationToken),
				//{ CallbackQuery: { } callbackQuery } => _callbackQueryHandler.HandleCallbackQueryAsync(callbackQuery,
				//	cancellationToken),
				//{ InlineQuery: { } inlineQuery } => _inlineQueryHandler.HandleInlineQueryAsync(inlineQuery,
				//	cancellationToken),
				//{ ChosenInlineResult: { } chosenInlineResult } => _chosenInlineResultHandler
				//	.HandleChosenInlineResultAsync(chosenInlineResult, cancellationToken),
				_ => Task.FromException(new NotSupportedException($"Unknown Update Type: {update.Type}"))
			};

			try
			{
				await handler;
			}
			catch (Exception e)
			{
				_logger.LogError("{Message}\n{StackTrace}", e.Message, e.StackTrace);
			}
		}
		else
		{
			_logger.LogWarning("Update is Null");
		}
	}
}