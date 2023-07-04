using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces;

public interface ICommandService
{
	void Initialize();
	bool IsCommand(string command);
	Task HandleTextCommandAsync(Message message, string command, CancellationToken cancellationToken);
}