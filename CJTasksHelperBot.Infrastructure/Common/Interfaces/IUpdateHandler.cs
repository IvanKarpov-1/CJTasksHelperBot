using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces;

public interface IUpdateHandler
{
	Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);
}