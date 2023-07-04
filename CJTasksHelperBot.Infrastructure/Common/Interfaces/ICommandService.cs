using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces;

public interface ICommandService
{
	bool IsCommand(string command);
	Task HandleTextCommandAsync(Message message, string command, CancellationToken cancellationToken);
}