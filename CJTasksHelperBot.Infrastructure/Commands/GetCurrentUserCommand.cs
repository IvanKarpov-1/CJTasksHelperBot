using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using Telegram.Bot;

namespace CJTasksHelperBot.Infrastructure.Commands;

public class GetCurrentUserCommand : ICommand
{
	private readonly ITelegramBotClient _botClient;

	public GetCurrentUserCommand(ITelegramBotClient botClient)
	{
		_botClient = botClient;
	}

	public CommandType.CommandType CommandType { get; set; } = Infrastructure.CommandType.CommandType.GetCurrentUserCommand;

	public async Task ExecuteAsync(UserDto userDto, ChatDto chatDto, CancellationToken cancellationToken)
	{
		await _botClient.SendTextMessageAsync(
			chatId: chatDto.TelegramId,
			text: $"{userDto.FirstName} {userDto.LastName}",
			cancellationToken: cancellationToken);
	}
}