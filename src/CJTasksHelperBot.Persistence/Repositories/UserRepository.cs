using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Domain.Entities;

namespace CJTasksHelperBot.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    private ApplicationDbContext ApplicationDbContext => (Context as ApplicationDbContext)!;
}