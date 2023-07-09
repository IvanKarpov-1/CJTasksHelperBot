using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace CJTasksHelperBot.Infrastructure.Commands;

public class StartCommand : ICommand
{
	private readonly ITelegramBotClient _botClient;

	public StartCommand(ITelegramBotClient botClient)
	{
		_botClient = botClient;
	}

	public CommandType CommandType { get; set; } = CommandType.StartCommand;

	public async Task ExecuteAsync(UserDto userDto, ChatDto chatDto, CancellationToken cancellationToken)
	{
		const string usage = "Використання:\n" +
		                     "/start\t\t- показати доступні команди\n" +
		                     "/get_current_user\t- отримати ім'я та фамілію поточного користувача\n";

		await _botClient.SendTextMessageAsync(
			chatId: chatDto.Id,
			text: usage,
			replyMarkup: new ReplyKeyboardRemove(),
			cancellationToken: cancellationToken);
	}
}