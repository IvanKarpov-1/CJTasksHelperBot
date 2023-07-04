using CJTasksHelperBot.Application.Common.Models;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces;

public interface IChatService
{
	public Task<ChatDto?> FindChatByIdAsync(long id);
	public Task<ChatDto> GetChatFromTelegramModelAsync(Chat chat);
}