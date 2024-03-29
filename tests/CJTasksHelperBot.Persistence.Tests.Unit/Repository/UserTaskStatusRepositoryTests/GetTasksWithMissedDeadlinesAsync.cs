using CJTasksHelperBot.Persistence.DataSeeding;
using CJTasksHelperBot.Persistence.Repositories;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Persistence.Tests.Unit.Repository.UserTaskStatusRepositoryTests;

public class GetTasksWithMissedDeadlinesAsync
{
    [Fact]
    public async Task GetTasksWithMissedDeadlinesAsync_WhenUserTaskStatusesAreNotFound_ReturnEmptyList()
    {
        // Arrange
        using var factory = new ApplicationDbContextFactory();
        await using var context = factory.CreateContext();
        var userTaskStatusRepository = new UserTaskStatusRepository(context);
        
        // Act
        var usesTaskStatuses = await userTaskStatusRepository.GetTasksWithMissedDeadlinesAsync();

        // Assert
        usesTaskStatuses.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetTasksWithMissedDeadlinesAsync_WhenUserTaskStatusesWithCompletedStatus_ReturnEmptyList()
    {
        // Arrange
        using var factory = new ApplicationDbContextFactory();
        await using var context = factory.CreateContext();
        var userTaskStatusRepository = new UserTaskStatusRepository(context);
        await DataSeeder.CreateSeeder(context)
            .AddUser()
            .WithTasks(3, DateTime.UtcNow.AddDays(-1), TaskStatus.Completed)
            .SeedAsync();
        
        // Act
        var usesTaskStatuses = await userTaskStatusRepository.GetTasksWithMissedDeadlinesAsync();

        // Assert
        usesTaskStatuses.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTasksWithMissedDeadlinesAsync_WhenUserTaskStatusesWithMissedDeadlineAreFound_ReturnUserTaskStatuses()
    {
        // Arrange
        using var factory = new ApplicationDbContextFactory();
        await using var context = factory.CreateContext();
        var userTaskStatusRepository = new UserTaskStatusRepository(context);
        await DataSeeder.CreateSeeder(context)
            .AddUser()
            .WithTasks(3, DateTime.UtcNow.AddDays(-1), TaskStatus.Completed)
            .AddUser()
            .WithTasks(2, DateTime.UtcNow.AddDays(-1))
            .AddUser()
            .WithTasks(1, DateTime.UtcNow.AddDays(1))
            .SeedAsync();
        
        // Act
        var usesTaskStatuses = await userTaskStatusRepository.GetTasksWithMissedDeadlinesAsync();
        
        // Assert
        usesTaskStatuses.Should().NotBeEmpty();
        usesTaskStatuses.Count.Should().Be(2);
    }
}