using CJTasksHelperBot.Infrastructure.Common.Interfaces.Handlers;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Handlers;

public class CallbackQueryHandler : ICallbackQueryHandler
{
	private readonly ICallbackQueryService _callbackQueryService;

	public CallbackQueryHandler(ICallbackQueryService callbackQueryService)
	{
		_callbackQueryService = callbackQueryService;
	}

	public async Task HandleCallbackQueryAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
	{
		await _callbackQueryService.HandleCallBackQueryAsync(callbackQuery, cancellationToken);
	}
}