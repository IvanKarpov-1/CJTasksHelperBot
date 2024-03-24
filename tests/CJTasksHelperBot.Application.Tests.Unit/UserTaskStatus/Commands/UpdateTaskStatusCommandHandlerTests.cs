using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.UserTaskStatus.Commands;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Application.Tests.Unit.UserTaskStatus.Commands;

public class UpdateTaskStatusCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnFailureResult_WhenSetTaskStatusDtoIsNull()
    {
        // Arrange
        var command = new UpdateTaskStatusCommand();
        var handler = new UpdateTaskStatusCommandHandler(_unitOfWork);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be("SetTaskStatusDto cannot be null");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnFailureResult_WhenPartialTaskIdIsNull()
    {
        // Arrange
        var setTaskStatusDtoFaker = new AutoFaker<SetTaskStatusDto>();
        var setTaskStatusDto = setTaskStatusDtoFaker
            .RuleFor(x => x.PartialTaskId, (_, _) => null)
            .Generate();
        var command = new UpdateTaskStatusCommand { SetTaskStatusDto = setTaskStatusDto };
        var handler = new UpdateTaskStatusCommandHandler(_unitOfWork);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be("Partial task ID cannot be null");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnFailureResult_WhenPartialTaskTitleIsNull()
    {
        // Arrange
        var setTaskStatusDtoFaker = new AutoFaker<SetTaskStatusDto>();
        var setTaskStatusDto = setTaskStatusDtoFaker
            .RuleFor(x => x.PartialTaskTitle, (_, _) => null)
            .Generate();
        var command = new UpdateTaskStatusCommand { SetTaskStatusDto = setTaskStatusDto };
        var handler = new UpdateTaskStatusCommandHandler(_unitOfWork);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be("Partial task title cannot be null");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnFailureResult_WhenUnableToGetUserTaskStatus()
    {
        // Arrange
        var faker = new AutoFaker();
        var setTaskStatusDto = faker.Generate<SetTaskStatusDto>();
        var command = new UpdateTaskStatusCommand { SetTaskStatusDto = setTaskStatusDto };
        var handler = new UpdateTaskStatusCommandHandler(_unitOfWork);
        _unitOfWork.UserTaskStatusRepository
            .GetUserTaskStatus(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(System.Threading.Tasks.Task.FromResult<Domain.Entities.UserTaskStatus?>(null));

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be("Something went wrong when trying to update Task Status");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnFailureResult_WhenTryingToUpdateTaskStatus()
    {
        // Arrange
        var faker = new AutoFaker();
        var setTaskStatusDto = faker.Generate<SetTaskStatusDto>();
        var userTaskStatus = faker.Generate<Domain.Entities.UserTaskStatus>();
        var command = new UpdateTaskStatusCommand { SetTaskStatusDto = setTaskStatusDto };
        var handler = new UpdateTaskStatusCommandHandler(_unitOfWork);
        _unitOfWork.UserTaskStatusRepository
            .GetUserTaskStatus(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(userTaskStatus);
        _unitOfWork.CommitAsync().Returns(0);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be("Something went wrong when trying to update Task Status");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_UpdateTaskStatus()
    {
        // Arrange
        var allowedStatuses = new[]
        {
            TaskStatus.NotStarted,
            TaskStatus.InProgress,
            TaskStatus.AlmostDone,
            TaskStatus.Completed,
        };
        
        var setTaskStatusDtoFaker = new AutoFaker<SetTaskStatusDto>();
        var setTaskStatusDto = setTaskStatusDtoFaker
            .RuleFor(x => x.TaskStatus, f => f.PickRandom(allowedStatuses))
            .Generate();

        var faker = new AutoFaker();
        var oldTaskStatus = faker.Generate<TaskStatus>();
        
        var userTaskStatusFaker = new AutoFaker<Domain.Entities.UserTaskStatus>();
        var userTaskStatus = userTaskStatusFaker
            .RuleFor(x => x.TaskStatus,
                oldTaskStatus != TaskStatus.DeadlineMissed ? oldTaskStatus : TaskStatus.NotStarted)
            .Generate();
        
        var command = new UpdateTaskStatusCommand { SetTaskStatusDto = setTaskStatusDto };
        var handler = new UpdateTaskStatusCommandHandler(_unitOfWork);
        _unitOfWork.UserTaskStatusRepository
            .GetUserTaskStatus(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(userTaskStatus);
        _unitOfWork.CommitAsync().Returns(1);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        userTaskStatus.TaskStatus.Should().Be(setTaskStatusDto.TaskStatus);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_UpdateTaskStatusToCompletedWithMissedDeadline_WhenOldStatusWasDeadlineMissedAndNewWasCompleted()
    {
        // Arrange
        var setTaskStatusDtoFaker = new AutoFaker<SetTaskStatusDto>();
        var setTaskStatusDto = setTaskStatusDtoFaker
            .RuleFor(x => x.TaskStatus, TaskStatus.Completed)
            .Generate();
        
        var userTaskStatusFaker = new AutoFaker<Domain.Entities.UserTaskStatus>();
        var userTaskStatus = userTaskStatusFaker
            .RuleFor(x => x.TaskStatus, TaskStatus.DeadlineMissed)
            .Generate();
        
        var command = new UpdateTaskStatusCommand { SetTaskStatusDto = setTaskStatusDto };
        var handler = new UpdateTaskStatusCommandHandler(_unitOfWork);
        _unitOfWork.UserTaskStatusRepository
            .GetUserTaskStatus(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(userTaskStatus);
        _unitOfWork.CommitAsync().Returns(1);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        userTaskStatus.TaskStatus.Should().Be(TaskStatus.CompletedWithMissedDeadline);
    }
}