using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces;

public interface ICommand
{
	Task ExecuteAsync(Message message);
}