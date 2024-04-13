using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CJTasksHelperBot.Infrastructure.Services;

public class CommandHelpService : ICommandHelpService
{
	private readonly ITelegramBotClient _botClient;
	private readonly ILogger<CommandHelpService> _logger;
	private readonly IStringLocalizer<Messages> _localizer;

	public CommandHelpService(ITelegramBotClient botClient, ILogger<CommandHelpService> logger, IStringLocalizer<Messages> localizer)
	{
		_botClient = botClient;
		_logger = logger;
		_localizer = localizer;
	}

	public async Task DisplayHelpAsync(long chatId, CommandType commandType, CancellationToken cancellationToken)
	{
		try
		{
			await _botClient.SendTextMessageAsync(
				chatId: chatId,
				text: _localizer[CommandHelp.FromValue(commandType.Id).DisplayName],
				parseMode: ParseMode.Html,
				cancellationToken: cancellationToken);
		}
		catch (Exception e)
		{
			_logger.LogError("{Exception}", e.ToString());
		}

	}
}