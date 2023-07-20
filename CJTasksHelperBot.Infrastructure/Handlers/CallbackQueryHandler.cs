using CJTasksHelperBot.Infrastructure.Common.Interfaces.Handlers;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Handlers;

public class CallbackQueryHandler : ICallbackQueryHandler
{
	private readonly ITelegramBotClient _botClient;
	private readonly ICallbackQueryService _callbackQueryService;

	public CallbackQueryHandler(ITelegramBotClient botClient, ICallbackQueryService callbackQueryService)
	{
		_botClient = botClient;
		_callbackQueryService = callbackQueryService;
	}

	public async Task HandleCallbackQueryAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
	{
		await _botClient.AnswerCallbackQueryAsync(
			callbackQueryId: callbackQuery.Id,
			text: callbackQuery.Data,
			cancellationToken: cancellationToken);

		await _callbackQueryService.HandleCallBackQueryAsync(callbackQuery, cancellationToken);
	}
}