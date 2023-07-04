using CJTasksHelperBot.Infrastructure.Common.Helpers;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Services;

public class CommandService : ICommandService
{
	private readonly IEnumerable<ICommand> _commands;

	public CommandService(IEnumerable<ICommand> commands)
	{
		_commands = commands;
	}

	public bool IsCommand(string command) => CommandType.CommandType.GetAll().Any(x => x.DisplayName == command);

	public Task HandleTextCommandAsync(Message message, string command, CancellationToken cancellationToken)
	{
		var botCommand = GetCommand(command);
		botCommand?.ExecuteAsync(message, cancellationToken);
		return Task.CompletedTask;
	}

	private ICommand? GetCommand(string command)
	{
		return _commands!.FirstOrDefault(x => x.CommandType == CommandType.CommandType.FromDisplayName(command), null);
	}
}