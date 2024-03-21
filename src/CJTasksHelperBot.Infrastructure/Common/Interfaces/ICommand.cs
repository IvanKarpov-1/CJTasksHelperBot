using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Enums;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces;

public interface ICommand
{
	CommandType CommandType { get; }
	bool IsAllowCommandLineArguments { get; }

	Task ExecuteAsync(UserDto userDto, ChatDto chatDto, CancellationToken cancellationToken);
	Task ExecuteWithCommandLineArguments(UserDto userDto, ChatDto chatDto, Dictionary<string, string> arguments, CancellationToken cancellationToken);
}