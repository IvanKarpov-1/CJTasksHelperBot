using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Telegram.Bot;

namespace CJTasksHelperBot.Infrastructure.Services;

public partial class CommandService : ICommandService
{
	private readonly IEnumerable<ICommand> _commands;
	private readonly ITelegramBotClient _botClient;

	private Regex? _commandRegex;
	private Regex? _commandLineArgumentsRegex;

	public CommandService(IEnumerable<ICommand> commands, ITelegramBotClient botClient)
	{
		_commands = commands;
		_botClient = botClient;
	}

	[GeneratedRegex("((?'parameter'[-]{1}[\\w]{1})|(?'parameter'[-]{2}[\\w]+))((?'argument' [^-]+)|())")]
	private static partial Regex GetCommandLineArgumentRegex();

	public async Task InitializeAsync()
	{
		if (_commandRegex != null) return;

		var bot = await _botClient.GetMeAsync();
		_commandRegex = new Regex($@"^\/[A-Za-z_]+(@{bot.Username})?", RegexOptions.IgnoreCase);

		_commandLineArgumentsRegex = GetCommandLineArgumentRegex();
	}

	public bool IsCommand(string command)
	{
		if (_commandRegex != null && !_commandRegex.IsMatch(command.ToLower()))
		{
			return false;
		}

		var parsedCommand = ParseCommand(_commandRegex!.Match(command).ToString());

		return CommandType.GetAll().Any(x => x.DisplayName == parsedCommand);
	}

	private static string ParseCommand(string command)
	{
		var isContainsAt = command.Contains('@');
		if (isContainsAt)
		{
			command = command.ToLower().Split('@')[0];
		}

		return command;
	}

	private bool IsCommandHaveCommandLineArguments(string command)
	{
		return _commandLineArgumentsRegex != null && _commandLineArgumentsRegex.IsMatch(command);
	}

	private Dictionary<string, string> ParseCommandLineArguments(string command)
	{
		var matches = _commandLineArgumentsRegex!.Matches(command);
		var arguments = new Dictionary<string, string>();
		foreach (var m in matches.Cast<Match>())
		{
			arguments[m.Groups["parameter"].Value] = m.Groups["argument"].Value;
		}

		return arguments;
	}

	public Task HandleTextCommandAsync(UserDto userDto, ChatDto chatDto, string command, CancellationToken cancellationToken)
	{
		var parsedCommand = ParseCommand(command);
		var botCommand = GetCommand(parsedCommand);
		if (botCommand is { IsAllowCommandLineArguments: true } && IsCommandHaveCommandLineArguments(command))
		{
			var arguments = ParseCommandLineArguments(command);
			botCommand.ExecuteWithCommandLineArguments(userDto, chatDto, arguments, cancellationToken);
		}
		botCommand?.ExecuteAsync(userDto, chatDto, cancellationToken);
		return Task.CompletedTask;
	}

	private ICommand? GetCommand(string command)
	{
		return _commands.FirstOrDefault(x => x.CommandType == CommandType.FromDisplayName(command), null);
	}
}