using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CJTasksHelperBot.Infrastructure.Services;

public class CommandHelpService : ICommandHelpService
{
	private readonly ITelegramBotClient _botClient;
	private readonly ILogger<CommandHelpService> _logger;

	public CommandHelpService(ITelegramBotClient botClient, ILogger<CommandHelpService> logger)
	{
		_botClient = botClient;
		_logger = logger;
	}

	public async Task DisplayHelpAsync(long chatId, CommandType commandType, CancellationToken cancellationToken)
	{
		try
		{
			await _botClient.SendTextMessageAsync(
				chatId: chatId,
				text: CommandHelp.FromValue(commandType.Id).DisplayName,
				parseMode: ParseMode.MarkdownV2,
				cancellationToken: cancellationToken);
		}
		catch (Exception e)
		{
			_logger.LogError("{Exception}", e.ToString());
		}

	}
}