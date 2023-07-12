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

	public CommandType CommandType => CommandType.StartCommand;
	public bool IsAllowCommandLineArguments => false;

	public async Task ExecuteAsync(UserDto userDto, ChatDto chatDto, CancellationToken cancellationToken)
	{
		const string usage = "Використання:\n" +
		                     "/start - показати доступні команди\n" +
		                     "/get_current_user - отримати ім'я та фамілію поточного користувача\n" +
		                     "/add_task - додати завдання\n";

		await _botClient.SendTextMessageAsync(
			chatId: chatDto.Id,
			text: usage,
			replyMarkup: new ReplyKeyboardRemove(),
			cancellationToken: cancellationToken);
	}

	public async Task ExecuteWithCommandLineArguments(UserDto userDto, ChatDto chatDto, Dictionary<string, string> arguments,
		CancellationToken cancellationToken)
	{
		await ExecuteAsync(userDto, chatDto, cancellationToken);
	}
}