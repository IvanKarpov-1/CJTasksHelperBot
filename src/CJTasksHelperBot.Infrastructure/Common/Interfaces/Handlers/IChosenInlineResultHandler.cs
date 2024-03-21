using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces.Handlers;

public interface IChosenInlineResultHandler
{
    Task HandleChosenInlineResultAsync(ChosenInlineResult chosenInlineResult, CancellationToken cancellationToken);
}