using CJTasksHelperBot.Application.Common.Models;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;

public interface IUserChatService
{
    Task<UserChatDto?> FindUserChatByIdsAsync(long userId, long chatId);
    Task CreateUserChatAsync(long userId, long chatId, bool checkExistence);
}