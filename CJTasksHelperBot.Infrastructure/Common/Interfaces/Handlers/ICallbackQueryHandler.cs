using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces.Handlers;

public interface ICallbackQueryHandler
{
    Task HandleCallbackQueryAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken);
}