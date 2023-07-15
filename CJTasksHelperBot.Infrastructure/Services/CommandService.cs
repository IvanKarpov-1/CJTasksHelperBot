using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using System.Text.RegularExpressions;
using CJTasksHelperBot.Infrastructure.Common.Extensions;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Telegram.Bot;

namespace CJTasksHelperBot.Infrastructure.Services;

public partial class CommandService : ICommandService
{
	private readonly IEnumerable<ICommand> _commands;
	private readonly ITelegramBotClient _botClient;
	private readonly ICommandHelpService _commandHelpService;

	private Regex? _commandRegex;
	private Regex? _commandLineArgumentsRegex;

	public CommandService(IEnumerable<ICommand> commands, ITelegramBotClient botClient, ICommandHelpService commandHelpService)
	{
		_commands = commands;
		_botClient = botClient;
		_commandHelpService = commandHelpService;
	}

	[GeneratedRegex("(?<parameter>(?<=\\s{1})([-]{1}[\\w]{1}(?=\\s))|(([-]{2}|[—]{1})[\\w]{2,}))(?<argument> ([^-—]+((?<=[^\\s])(-+[^-—]+))*|[^-—]+)|)")]
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

		var parsedCommand = ParseCommand(command);

		return CommandType.GetAll().Any(x => x.DisplayName == parsedCommand);
	}

	private string ParseCommand(string command)
	{
		command = _commandRegex!.Match(command).ToString();
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
			var parameter = m.Groups["parameter"].Value.Replace("—", "--");
			arguments[parameter] = m.Groups["argument"].Value.Trim();
		}

		return arguments;
	}

	public async Task HandleTextCommandAsync(UserDto userDto, ChatDto chatDto, string command, CancellationToken cancellationToken)
	{
		command += " ";
		var parsedCommand = ParseCommand(command);
		var botCommand = GetCommand(parsedCommand);
		if (botCommand == null) return;
		if (botCommand.IsAllowCommandLineArguments && IsCommandHaveCommandLineArguments(command))
		{
			var arguments = ParseCommandLineArguments(command);
			var isContainsHelp = arguments.ContainsKey(CommandLineArgument.Help);
			if (isContainsHelp)
			{
				await _commandHelpService.DisplayHelpAsync(chatDto.Id, CommandType.FromDisplayName(parsedCommand), cancellationToken);
				return;
			}

			await botCommand.ExecuteWithCommandLineArguments(userDto, chatDto, arguments, cancellationToken);
			return;
		}

		await botCommand.ExecuteAsync(userDto, chatDto, cancellationToken);
	}

	private ICommand? GetCommand(string command)
	{
		return _commands.FirstOrDefault(x => x.CommandType == CommandType.FromDisplayName(command), null);
	}
}