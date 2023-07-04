using CJTasksHelperBot.Application.Common.Models;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces;

public interface ICommand
{
	CommandType.CommandType CommandType { get; set; }
	Task ExecuteAsync(UserDto userDto, ChatDto chatDto, CancellationToken cancellationToken);
}