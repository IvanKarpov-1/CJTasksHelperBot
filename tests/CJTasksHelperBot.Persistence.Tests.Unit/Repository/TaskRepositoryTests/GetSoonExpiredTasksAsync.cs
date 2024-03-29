using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Persistence.DataSeeding;
using CJTasksHelperBot.Persistence.Repositories;

namespace CJTasksHelperBot.Persistence.Tests.Unit.Repository.TaskRepositoryTests;

public class GetSoonExpiredTasksAsync
{
    [Fact]
    public async Task GetSoonExpiredTasksAsync_WhenNoTasks_ReturnEmptyList()
    {
        // Arrange
        using var factory = new ApplicationDbContextFactory();
        await using var context = factory.CreateContext();
        var taskRepository = new TaskRepository(context);

        // Act
        var tasks = await taskRepository.GetSoonExpiredTasksAsync();

        // Assert
        tasks.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSoonExpiredTasksAsync_WhenNoSoonExpiredTasks_ReturnEmptyList()
    {
        // Arrange
        using var factory = new ApplicationDbContextFactory();
        await using var context = factory.CreateContext();
        var taskRepository = new TaskRepository(context);
        await DataSeeder.CreateSeeder(context)
            .AddUser()
            .WithTasks(3, deadline: DateTime.UtcNow.AddDays(-1))
            .SeedAsync();
        
        // Act
        var tasks = await taskRepository.GetSoonExpiredTasksAsync();

        // Assert
        tasks.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSoonExpiredTasksAsync_WhenAllTasksWithNotLevelNever_ReturnEmptyList()
    {
        // Arrange
        using var factory = new ApplicationDbContextFactory();
        await using var context = factory.CreateContext();
        var taskRepository = new TaskRepository(context);
        await DataSeeder.CreateSeeder(context)
            .AddUser()
            .WithTasks(3, notificationLevel: NotificationLevel.Never)
            .SeedAsync();
        
        // Act
        var tasks = await taskRepository.GetSoonExpiredTasksAsync();

        // Assert
        tasks.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSoonExpiredTasksAsync_WhenThereIsSoonExpiredTasks_ReturnSoonExpiredTasks()
    {
        // Arrange
        using var factory = new ApplicationDbContextFactory();
        await using var context = factory.CreateContext();
        var taskRepository = new TaskRepository(context);
        await DataSeeder.CreateSeeder(context)
            .AddUser()
            .WithTasks(1, DateTime.UtcNow.AddDays(1))
            .AddUser()
            .WithTasks(1, DateTime.UtcNow.AddDays(3), notificationLevel: NotificationLevel.Day)
            .AddUser()
            .WithTasks(1, DateTime.UtcNow.AddDays(1), notificationLevel: NotificationLevel.Never)
            .SeedAsync();
        
        // Act
        var tasks = await taskRepository.GetSoonExpiredTasksAsync();
        
        // Assert
        tasks.Should().NotBeEmpty();
        tasks.Count.Should().Be(1);
    }
}