using CJTasksHelperBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CJTasksHelperBot.Application.Common.Interfaces;

public interface IApplicationDbContext
{
	DbSet<Domain.Entities.Chat> Chats { get; }
	DbSet<Homework> Homework { get; }
	DbSet<Domain.Entities.Task> Tasks { get; }
	DbSet<Domain.Entities.User> Users { get; }
	DbSet<Domain.Entities.UserChat> UserChats { get; }
	DbSet<Domain.Entities.UserTaskStatus> UserTaskStatuses { get; }
	Task<int> SaveChangeAsync(CancellationToken cancellationToken);
}