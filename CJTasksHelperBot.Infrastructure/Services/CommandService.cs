using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using System.Text.RegularExpressions;
using Telegram.Bot;

namespace CJTasksHelperBot.Infrastructure.Services;

public class CommandService : ICommandService
{
	private readonly IEnumerable<ICommand> _commands;
	private readonly ITelegramBotClient _botClient;

	private Regex? _commandRegex;

	public CommandService(IEnumerable<ICommand> commands, ITelegramBotClient botClient)
	{
		_commands = commands;
		_botClient = botClient;
	}
	
	public async Task InitializeAsync()
	{
		if (_commandRegex != null) return;

		var bot = await _botClient.GetMeAsync();
		_commandRegex = new Regex($@"^\/[A-Za-z_]+(@{bot.Username})?$", RegexOptions.IgnoreCase);
	}

	public bool IsCommand(string command)
	{
		if (_commandRegex != null && !_commandRegex.IsMatch(command.ToLower()))
		{
			return false;
		}

		var parsedCommand = ParseCommand(command);

		return CommandType.CommandType.GetAll().Any(x => x.DisplayName == parsedCommand);
	}

	private string ParseCommand(string command)
	{
		var isContainsAt = command.Contains('@');
		if (isContainsAt)
		{
			command = command.ToLower().Split('@')[0];
		}

		return command;
	}

	public Task HandleTextCommandAsync(UserDto userDto, ChatDto chatDto, string command, CancellationToken cancellationToken)
	{
		var parsedCommand = ParseCommand(command);
		var botCommand = GetCommand(parsedCommand);
		botCommand?.ExecuteAsync(userDto, chatDto, cancellationToken);
		return Task.CompletedTask;
	}

	private ICommand? GetCommand(string command)
	{
		return _commands!.FirstOrDefault(x => x.CommandType == CommandType.CommandType.FromDisplayName(command), null);
	}
}