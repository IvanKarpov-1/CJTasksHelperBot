using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Enums;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces;

public interface ICommand
{
	CommandType CommandType { get; set; }
	Task ExecuteAsync(UserDto userDto, ChatDto chatDto, CancellationToken cancellationToken);
}