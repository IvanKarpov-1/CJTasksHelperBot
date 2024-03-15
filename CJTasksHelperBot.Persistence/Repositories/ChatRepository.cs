using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CJTasksHelperBot.Persistence.Repositories;

public class ChatRepository : GenericRepository<Chat>, IChatRepository
{
    public ChatRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Chat>> GetChatsWithTasksAsync(long userId, CancellationToken cancellationToken)
    {
        // var chats = await (from userChat in ApplicationDbContext.UserChats
        //         join chat in ApplicationDbContext.Chats on userChat.ChatId equals chat.Id
        //         join task in ApplicationDbContext.Tasks on chat.Id equals task.UserChat!.ChatId
        //         where userChat.UserId == userId && task.UserChat!.ChatId != userId
        //         select new Chat
        //             {
        //                 Id = chat.Id,
        //                 Title = chat.Title,
        //                 CreatedAt = chat.CreatedAt,
        //                 LanguageCode = chat.LanguageCode
        //             })
        //     .GroupBy(x => x.Id)
        //     .Select(x => new Chat{Id = x.Key, Title = x.First().Title })
        //     .ToListAsync(cancellationToken);
        
        var chats = await ApplicationDbContext.UserChats
            .Join(ApplicationDbContext.Chats,
                userChat => userChat.ChatId,
                chat => chat.Id,
                (userChat, chat) => new { UserChat = userChat, Chat = chat })
            .Join(ApplicationDbContext.Tasks,
                uc => uc.Chat.Id,
                task => task.UserChat!.ChatId,
                (uc, task) => new { UserChat = uc.UserChat, Chat = uc.Chat, Task = task })
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