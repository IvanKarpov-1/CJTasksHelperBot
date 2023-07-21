using CJTasksHelperBot.Application.Common.Models;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces;

public interface ICallbackQuery
{
	bool IsRequireCallbackQuery { get; }
	void SetCallbackQuery(CallbackQuery callbackQuery);
	Task ExecuteAsync(UserDto userDto, ChatDto chatDto, Dictionary<string, object> data, CancellationToken cancellationToken);
}