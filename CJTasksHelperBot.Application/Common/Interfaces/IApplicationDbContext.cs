using CJTasksHelperBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Task = CJTasksHelperBot.Domain.Entities.Task;

namespace CJTasksHelperBot.Application.Common.Interfaces;

public interface IApplicationDbContext
{
	DbSet<Chat> Chats { get; }
	DbSet<Homework> Homework { get; }
	DbSet<Task> Tasks { get; }
	DbSet<Domain.Entities.User> Users { get; }
	DbSet<UserChat> UserChats { get; }
	Task<int> SaveChangeAsync(CancellationToken cancellationToken);
}