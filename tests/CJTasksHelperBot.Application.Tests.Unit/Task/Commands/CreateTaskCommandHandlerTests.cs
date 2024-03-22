using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.Task.Commands;
using CJTasksHelperBot.Domain.Enums;

namespace CJTasksHelperBot.Application.Tests.Unit.Task.Commands;

public class CreateTaskCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly MapperlyMapper _mapper;

    public CreateTaskCommandHandlerTests()
    {
        _mapper = new MapperlyMapper();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnFailureResult_WhenTaskDtoIsNull()
    {
        // Arrange
        var command = new CreateTaskCommand();
        var handler = new CreateTaskCommandHandler(_unitOfWork, new MapperlyMapper());

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be("TaskDto is null");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnFailureResult_WhenCannotAddTask()
    {
        // Arrange
        var faker = new AutoFaker();
        var createTaskDto = faker.Generate<CreateTaskDto>();
        var command = new CreateTaskCommand { CreateTaskDto = createTaskDto };
        var handler = new CreateTaskCommandHandler(_unitOfWork, new MapperlyMapper());
        _unitOfWork.CommitAsync().Returns(0);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be("Something went wrong whet trying to create Task");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnSuccessfulResult_WhenTaskIsSuccessfullyAdded()
    {
        // Arrange
        var faker = new AutoFaker();
        var createTaskDto = faker.Generate<CreateTaskDto>();
        
        var task = _mapper.Map(createTaskDto);
        task.Deadline = DateTime.MinValue;
        
        var user = _mapper.Map(createTaskDto.UserChatDto!.UserDto!);
        var userTaskStatus = new Domain.Entities.UserTaskStatus { Task = task, User = user };
        
        task.UserTaskStatuses.Add(userTaskStatus);
        
        var command = new CreateTaskCommand { CreateTaskDto = createTaskDto };
        var handler = new CreateTaskCommandHandler(_unitOfWork, new MapperlyMapper());
        
        _unitOfWork.CommitAsync().Returns(1);
        
        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWork.TaskRepository.Received().Attach(Arg.Any<Domain.Entities.Task>());
        await _unitOfWork.Received().CommitAsync();
    }

    public static IEnumerable<object[]> Deadlines => new List<object[]>
    {
        new object[] { DateTime.UtcNow, NotificationLevel.Never },
        new object[] { DateTime.UtcNow.AddDays(1), NotificationLevel.Never },
        new object[] { DateTime.UtcNow.AddDays(2), NotificationLevel.Day },
        new object[] { DateTime.UtcNow.AddDays(3), NotificationLevel.TwoDays },
        new object[] { DateTime.UtcNow.AddDays(8), NotificationLevel.Week },
    };

    [Theory]
    [MemberData(nameof(Deadlines))]
    public async System.Threading.Tasks.Task Handle_Should_SetAppropriateNotificationLevel(DateTime deadline,
        NotificationLevel notificationLevel)
    {
        // Arrange
        var faker = new AutoFaker();
        var createTaskDto = faker.Generate<CreateTaskDto>();
        createTaskDto.Deadline = deadline;
        var command = new CreateTaskCommand { CreateTaskDto = createTaskDto };
        var handler = new CreateTaskCommandHandler(_unitOfWork, new MapperlyMapper());

        // Act
        await handler.Handle(command, default);

        // Assert
        _unitOfWork.TaskRepository.Received().Attach(Arg.Is<Domain.Entities.Task>(t => t.NotificationLevel == notificationLevel));
    }
}