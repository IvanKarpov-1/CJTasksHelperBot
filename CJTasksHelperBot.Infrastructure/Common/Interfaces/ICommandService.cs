using CJTasksHelperBot.Application.Common.Models;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces;

public interface ICommandService
{
	Task InitializeAsync();
	bool IsCommand(string command);
	Task HandleTextCommandAsync(UserDto userDto, ChatDto chatDto, string command, CancellationToken cancellationToken);
}