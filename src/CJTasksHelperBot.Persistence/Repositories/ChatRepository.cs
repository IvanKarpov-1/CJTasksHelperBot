using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CJTasksHelperBot.Persistence.Repositories;

public class ChatRepository(ApplicationDbContext context) : GenericRepository<Chat>(context), IChatRepository
{
    public async Task<List<Chat>> GetChatsWithTasksAsync(long userId, CancellationToken cancellationToken)
    {
        var chats = await ApplicationDbContext.UserChats
            .Join(ApplicationDbContext.Chats,
                userChat => userChat.ChatId,
                chat => chat.Id,
                (userChat, chat) => new { UserChat = userChat, Chat = chat })
            .Join(ApplicationDbContext.Tasks,
                uc => uc.Chat.Id,
                task => task.UserChat!.ChatId,
                (uc, task) => new { uc.UserChat, uc.Chat, Task = task })
            .Where(joined => joined.UserChat.UserId == userId && joined.Task.UserChat!.ChatId != userId)
            .Select(joined => new Chat
            {
                Id = joined.Chat.Id,
                Title = joined.Chat.Title,
                CreatedAt = joined.Chat.CreatedAt,
                LanguageCode = joined.Chat.LanguageCode
            })
            .GroupBy(x => x.Id)
            .Select(group => new Chat
            {
                Id = group.Key,
                Title = group.First().Title
            })
            .ToListAsync(cancellationToken);

        return chats;
    }

    private ApplicationDbContext ApplicationDbContext => (Context as ApplicationDbContext)!;
}