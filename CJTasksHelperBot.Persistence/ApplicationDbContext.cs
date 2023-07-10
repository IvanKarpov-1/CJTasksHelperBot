using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Task = CJTasksHelperBot.Domain.Entities.Task;

namespace CJTasksHelperBot.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{
	}

	public DbSet<Chat> Chats => Set<Chat>();
	public DbSet<Homework> Homework => Set<Homework>();
	public DbSet<Task> Tasks => Set<Task>();
	public DbSet<User> Users => Set<User>();
	public DbSet<UserChat> UserChats => Set<UserChat>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		base.OnModelCreating(modelBuilder);
	}

	public Task<int> SaveChangeAsync(CancellationToken cancellationToken = default)
	{
		return base.SaveChangesAsync(cancellationToken);
	}
}