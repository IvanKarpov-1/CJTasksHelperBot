using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces.Handlers;

public interface IUpdateHandler
{
    Task HandleUpdateAsync(Update? update, CancellationToken cancellationToken);
}