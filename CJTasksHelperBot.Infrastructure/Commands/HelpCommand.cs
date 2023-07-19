using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Extensions;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CJTasksHelperBot.Infrastructure.Commands;

public class HelpCommand : ICommand
{
	private readonly ITelegramBotClient _botClient;

	public HelpCommand(ITelegramBotClient botClient)
	{
		_botClient = botClient;
	}

	public CommandType CommandType => CommandType.Help;
	public bool IsAllowCommandLineArguments => false;


	public async Task ExecuteAsync(UserDto userDto, ChatDto chatDto, CancellationToken cancellationToken)
	{
		await _botClient.SendTextMessageAsync(
			chatId: chatDto.Id,
			text: "Привіт\\! Я — CJTasksHelperBot\\. Я допоможу організувати ваші задачі та домашні завдання\\.\n\n" +
			      "Деякі команди виконуються в декілька кроків, тому потрібно не переривати ввід потрібної інформації або ж скасувати виконання команди\\.\n\n" +
			      "Деякі команди підтримують аргументи командного рядка з прапорцями\\. " +
			      $"Для деталей додайте прапорець `{CommandLineArgument.Help.DisplayName.EscapeCharacters()}` до команди \\(через пробіл\\)\\. " +
			      "Даний прапорець має пріоритет над іншими, тому інші буде проігноровано\\.\n\n" +
			      "Щоб взаємодіяти зі мною, використовуйте такі команди:\n" +
			      $"{CommandType.Help.DisplayName.EscapeCharacters()} \\- показати цю підказку\n" +
			      $"{CommandType.AddTask.DisplayName.EscapeCharacters()} \\- додати завдання\n" +
			      "\n" +
			      $"{CommandStep.Stop.DisplayName.EscapeCharacters()} \\- зупинити поточну команду",
			parseMode: ParseMode.MarkdownV2,
			cancellationToken: cancellationToken);
	}

	public async Task ExecuteWithCommandLineArguments(UserDto userDto, ChatDto chatDto, Dictionary<string, string> arguments,
		CancellationToken cancellationToken)
	{
		await ExecuteAsync(userDto, chatDto, cancellationToken);
	}
}