using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces.Handlers;

public interface IMessageHandler
{
    Task HandleMessageAsync(Message message, CancellationToken cancellationToken);
}