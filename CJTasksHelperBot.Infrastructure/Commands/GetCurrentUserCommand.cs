using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Enums;
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

	public CommandType CommandType => CommandType.GetCurrentUserCommand;
	public bool IsAllowCommandLineArguments => false;

	public async Task ExecuteAsync(UserDto userDto, ChatDto chatDto, CancellationToken cancellationToken)
	{
		await _botClient.SendTextMessageAsync(
			chatId: chatDto.Id,
			text: $"{userDto.FirstName} {userDto.LastName}",
			cancellationToken: cancellationToken);
	}
	
	public async Task ExecuteWithCommandLineArguments(UserDto userDto, ChatDto chatDto, Dictionary<string, string> arguments,
		CancellationToken cancellationToken)
	{
		await ExecuteAsync(userDto, chatDto, cancellationToken);
	}
}