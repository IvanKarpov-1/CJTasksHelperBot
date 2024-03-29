using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Domain.Entities;

namespace CJTasksHelperBot.Persistence.Repositories;

public class UserChatRepository(ApplicationDbContext context)
    : GenericRepository<UserChat>(context), IUserChatRepository;