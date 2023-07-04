using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces;

public interface ICommand
{
	CommandType.CommandType CommandType { get; set; }
	Task ExecuteAsync(Message message, CancellationToken cancellationToken);
}