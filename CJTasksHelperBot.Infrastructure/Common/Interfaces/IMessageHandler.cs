using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces;

public interface IMessageHandler
{
	Task HandleMessageAsync(Message message, CancellationToken cancellationToken);
}