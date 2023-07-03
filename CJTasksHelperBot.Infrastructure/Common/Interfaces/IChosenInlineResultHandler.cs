using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces;

public interface IChosenInlineResultHandler
{
	Task HandleChosenInlineResultAsync(ChosenInlineResult chosenInlineResult, CancellationToken cancellationToken);
}