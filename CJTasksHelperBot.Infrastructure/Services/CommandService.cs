using CJTasksHelperBot.Application.Common.Models;
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

	public Task HandleTextCommandAsync(UserDto userDto, ChatDto chatDto, string command, CancellationToken cancellationToken)
	{
		var botCommand = GetCommand(command);
		botCommand?.ExecuteAsync(userDto, chatDto, cancellationToken);
		return Task.CompletedTask;
	}

	private ICommand? GetCommand(string command)
	{
		return _commands!.FirstOrDefault(x => x.CommandType == CommandType.CommandType.FromDisplayName(command), null);
	}
}