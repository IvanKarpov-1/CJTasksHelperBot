using CJTasksHelperBot.Infrastructure.Common.Enums;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;

public interface ICommandHelpService
{
	Task DisplayHelpAsync(long chatId, CommandType commandType, CancellationToken cancellationToken);
}