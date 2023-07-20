using CJTasksHelperBot.Application.Common.Models;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;

public interface ICallbackQueryService
{
	string CreateQuery<T>(UserDto user, params (string key, object value)[] args) where T : ICallbackQuery;
	Task HandleCallBackQueryAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken);
}