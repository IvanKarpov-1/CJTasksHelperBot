using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Domain.Entities;

namespace CJTasksHelperBot.Persistence.Repositories;

public class UserChatRepository : GenericRepository<UserChat>, IUserChatRepository
{
    public UserChatRepository(ApplicationDbContext context) : base(context)
    {
    }

    private ApplicationDbContext ApplicationDbContext => (Context as ApplicationDbContext)!;
}