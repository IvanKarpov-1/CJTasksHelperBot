using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Commands;

public class GetCurrentUserCommand : ICommand
{
	private readonly ITelegramBotClient _botClient;

	public GetCurrentUserCommand(ITelegramBotClient botClient)
	{
		_botClient = botClient;
		CommandType = Infrastructure.CommandType.CommandType.GetCurrentUserCommand;
	}

	public CommandType.CommandType CommandType { get; set; }

	public async Task ExecuteAsync(Message message, CancellationToken cancellationToken)
	{
		var user = message.From;

		await _botClient.SendTextMessageAsync(
			chatId: message.Chat.Id,
			text: $"{user.FirstName} {user.LastName}",
			cancellationToken: cancellationToken);
	}
}