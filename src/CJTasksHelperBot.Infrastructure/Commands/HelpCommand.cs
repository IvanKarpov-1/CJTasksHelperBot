using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CJTasksHelperBot.Infrastructure.Commands;

public class HelpCommand : ICommand
{
	private readonly ITelegramBotClient _botClient;
	private readonly IStringLocalizer<Messages> _localizer;

	public HelpCommand(ITelegramBotClient botClient, IStringLocalizer<Messages> localizer)
	{
		_botClient = botClient;
		_localizer = localizer;
	}

	public CommandType CommandType => CommandType.Help;
	public bool IsAllowCommandLineArguments => false;

	public async Task ExecuteAsync(UserDto userDto, ChatDto chatDto, CancellationToken cancellationToken)
	{
		await _botClient.SendTextMessageAsync(
			chatId: chatDto.Id,
			text: _localizer["help_message"],
			parseMode: ParseMode.Html,
			cancellationToken: cancellationToken);
	}

	public async Task ExecuteWithCommandLineArguments(UserDto userDto, ChatDto chatDto, Dictionary<string, string> arguments,
		CancellationToken cancellationToken)
	{
		await ExecuteAsync(userDto, chatDto, cancellationToken);
	}
}