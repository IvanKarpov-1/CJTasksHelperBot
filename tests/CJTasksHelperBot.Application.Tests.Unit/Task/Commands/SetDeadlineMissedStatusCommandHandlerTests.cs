using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Task.Commands;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Application.Tests.Unit.Task.Commands;

public class SetDeadlineMissedStatusCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenGetTaskWithMissedDeadlines_SetMissedDeadlineStatusToIt()
    {
        // Arrange
        var faker = new AutoFaker();
        var tasks = faker.Generate<Domain.Entities.UserTaskStatus>(5);
        tasks[0].TaskStatus = TaskStatus.Completed;
        var command = new SetDeadlineMissedStatusCommand();
        var handler = new SetDeadlineMissedStatusCommandHandler(_unitOfWork);
        _unitOfWork.UserTaskStatusRepository.GetTasksWithMissedDeadlinesAsync().Returns(tasks);
        
        // Act
        await handler.Handle(command, default);

        // Assert
        foreach (var userTaskStatus in tasks)
        {
            userTaskStatus.TaskStatus.Should().Be(TaskStatus.DeadlineMissed);
        }
    }
}