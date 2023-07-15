using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CJTasksHelperBot.Infrastructure.Services;

public class CommandHelpService : ICommandHelpService
{
	private readonly ITelegramBotClient _botClient;

	public CommandHelpService(ITelegramBotClient botClient)
	{
		_botClient = botClient;
	}

	public async Task DisplayHelpAsync(long chatId, CommandType commandType, CancellationToken cancellationToken)
	{
		await _botClient.SendTextMessageAsync(
			chatId: chatId,
			text: CommandHelp.FromValue(commandType.Id).DisplayName,
			parseMode: ParseMode.MarkdownV2,
			cancellationToken: cancellationToken);
	}
}