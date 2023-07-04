using CJTasksHelperBot.Application.Common.Models;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;

public interface IChatService
{
    public Task<ChatDto?> FindChatByIdAsync(long id);
    public Task<ChatDto> GetChatFromTelegramModelAsync(Chat chat);
}