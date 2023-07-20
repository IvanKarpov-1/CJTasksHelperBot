using CJTasksHelperBot.Application.Common.Models;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces;

public interface ICallbackQuery
{
	Task ExecuteAsync(UserDto userDto, ChatDto chatDto, Dictionary<string, object> data, CancellationToken cancellationToken);
}