using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Persistence.DataSeeding;
using CJTasksHelperBot.Persistence.Repositories;

namespace CJTasksHelperBot.Persistence.Tests.Unit.Repository.TaskRepositoryTests;

public class GetTasksFromChatAsync
{
    [Fact]
    public async Task GetTasksFromChatAsync_WhenChatIsNotFound_ReturnEmptyList()
    {
        // Arrange
        using var factory = new ApplicationDbContextFactory();
        await using var context = factory.CreateContext();
        var taskRepository = new TaskRepository(context);
        
        // Act
        var tasks = await taskRepository.GetTasksFromChatAsync(0);

        // Assert
        tasks.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetTasksFromChatAsync_WhenTasksInCatAreNotFound_ReturnEmptyList()
    {
        // Arrange
        using var factory = new ApplicationDbContextFactory();
        await using var context = factory.CreateContext();
        var taskRepository = new TaskRepository(context);
        await DataSeeder.CreateSeeder(context)
            .AddChats(0)
            .SeedAsync();
        
        // Act
        var tasks = await taskRepository.GetTasksFromChatAsync(0);

        // Assert
        tasks.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetTasksFromChatAsync_WhenTasksInCatAreFound_ReturnTasks()
    {
        // Arrange
        using var factory = new ApplicationDbContextFactory();
        await using var context = factory.CreateContext();
        var taskRepository = new TaskRepository(context);
        await DataSeeder.CreateSeeder(context)
            .AddChats(0)
            .AddUser()
            .WithTasks(3, notificationLevel: NotificationLevel.Never)
            .AddChats(1)
            .AddUser(1)
            .WithTasks(4, notificationLevel: NotificationLevel.TwoDays)
            .SeedAsync();
        
        // Act
        var tasks = await taskRepository.GetTasksFromChatAsync(0);

        // Assert
        tasks.Should().NotBeEmpty();
        tasks.Count.Should().Be(3);
        tasks.ForEach(task => task.NotificationLevel.Should().Be(NotificationLevel.Never));
    }
}