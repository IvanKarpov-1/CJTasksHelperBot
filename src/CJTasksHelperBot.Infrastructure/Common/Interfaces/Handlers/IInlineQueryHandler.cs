using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces.Handlers;

public interface IInlineQueryHandler
{
    Task HandleInlineQueryAsync(InlineQuery inlineQuery, CancellationToken cancellationToken);
}