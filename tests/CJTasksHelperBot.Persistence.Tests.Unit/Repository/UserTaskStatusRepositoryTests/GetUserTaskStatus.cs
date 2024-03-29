using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Persistence.DataSeeding;
using CJTasksHelperBot.Persistence.Repositories;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Persistence.Tests.Unit.Repository.UserTaskStatusRepositoryTests;

public class GetUserTaskStatus
{
    [Fact]
    public async Task GetUserTaskStatus_WhenUserTaskStatusIsNotFound_ReturnNull()
    {
        // Arrange
        using var factory = new ApplicationDbContextFactory();
        await using var context = factory.CreateContext();
        var userTaskStatusRepository = new UserTaskStatusRepository(context);
        
        // Act
        var usesTaskStatus = await userTaskStatusRepository.GetUserTaskStatus(0, "", "");

        // Assert
        usesTaskStatus.Should().BeNull();
    }
    
    [Fact]
    public async Task GetUserTaskStatus_WhenUserTaskStatusIsFound_ReturnUserTaskStatus()
    {
        // Arrange
        const long userId = 1;
        const TaskStatus taskStatus = TaskStatus.Completed;
        const NotificationLevel notificationLevel = NotificationLevel.TwoDays;
        
        using var factory = new ApplicationDbContextFactory();
        await using var context = factory.CreateContext();
        var userTaskStatusRepository = new UserTaskStatusRepository(context);
        await DataSeeder.CreateSeeder(context)
            .AddUser(0, userId)
            .WithTasks(taskStatus: taskStatus, notificationLevel: notificationLevel)
            .AddUser(0, userId)
            .WithTasks(taskStatus: TaskStatus.NotStarted, notificationLevel: NotificationLevel.Week)
            .SeedAsync();
        
        var task = context.Tasks.First(x => x.NotificationLevel == notificationLevel);
        var partialTaskId = task.Id.ToString()[..4];
        var partialTaskTitle = task.Title![..4];
        
        // Act
        var usesTaskStatus = await userTaskStatusRepository.GetUserTaskStatus(userId, partialTaskId, partialTaskTitle);

        // Assert
        usesTaskStatus.Should().NotBeNull();
        usesTaskStatus!.TaskStatus.Should().Be(taskStatus);
        usesTaskStatus.Task.Should().Be(task);
    }
}