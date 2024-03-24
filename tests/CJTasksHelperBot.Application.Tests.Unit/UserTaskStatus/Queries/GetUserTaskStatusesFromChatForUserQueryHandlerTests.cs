using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.UserTaskStatus.Queries;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Application.Tests.Unit.UserTaskStatus.Queries;

public class GetUserTaskStatusesFromChatForUserQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly MapperlyMapper _mapper;

    public GetUserTaskStatusesFromChatForUserQueryHandlerTests()
    {
        _mapper = new MapperlyMapper();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnFailureResult_WhenChatIdIsNull()
    {
        // Arrange
        var query = new GetUserTaskStatusesFromChatForUserQuery();
        var handler = new GetUserTaskStatusesFromChatForUserQueryHandler(_unitOfWork, _mapper);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be("ChatId must not be null");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnUserTaskStatusDtos_WhenTasksForChatAreFound()
    {
        // Arrange
        const int countToGenerate = 5;
        
        var faker = new AutoFaker();
        var chatId = faker.Generate<long>();
        var userId = faker.Generate<long>();

        var userTaskStatusFaker = new AutoFaker<Domain.Entities.UserTaskStatus>();
        var userTaskStatuses = userTaskStatusFaker
            .RuleFor(x => x.UserId, userId)
            .RuleFor(x => x.Task, (_, _) => null)
            .Generate(countToGenerate);

        var taskFaker = new AutoFaker<Domain.Entities.Task>();
        var tasks = taskFaker
            .RuleFor(x => x.UserTaskStatuses, 
                f => new List<Domain.Entities.UserTaskStatus> { userTaskStatuses[f.IndexFaker] })
            .Generate(countToGenerate);

        for (var i = 0; i < countToGenerate; i++)
        {
            userTaskStatuses[i].Task = tasks[i];
        }

        var userTaskStatusDtos = userTaskStatuses.Select(_mapper.Map);
        
        var query = new GetUserTaskStatusesFromChatForUserQuery { ChatId = chatId, UserId = userId };
        var handler = new GetUserTaskStatusesFromChatForUserQueryHandler(_unitOfWork, _mapper);
        _unitOfWork.TaskRepository.GetTasksFromChatAsync(Arg.Is<long>(x => x == chatId)).Returns(tasks);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(userTaskStatusDtos);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_AddUserTaskStatusToTask_IfThereAreNoUserTaskStatusesForUser()
    {
        // Arrange
        const int countToGenerate = 5;
        
        var faker = new AutoFaker();
        var chatId = faker.Generate<long>();
        var userId = faker.Generate<long>();

        var taskFaker = new AutoFaker<Domain.Entities.Task>();
        var tasks = taskFaker
            .RuleFor(x => x.UserTaskStatuses, (_, _) => new List<Domain.Entities.UserTaskStatus>())
            .Generate(countToGenerate);
        
        var query = new GetUserTaskStatusesFromChatForUserQuery { ChatId = chatId, UserId = userId };
        var handler = new GetUserTaskStatusesFromChatForUserQueryHandler(_unitOfWork, _mapper);
        _unitOfWork.TaskRepository.GetTasksFromChatAsync(Arg.Is<long>(x => x == chatId)).Returns(tasks);
        
        // Act
        var result = await handler.Handle(query, default);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        foreach (var task in tasks)
        {
            task.UserTaskStatuses.First().UserId.Should().Be(userId);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnReturnUserTaskStatusDtos_WithSpecificTaskStatus()
    {
        // Arrange
        const int countToGenerate = 5;
        
        var faker = new AutoFaker();
        var chatId = faker.Generate<long>();
        var userId = faker.Generate<long>();
        var desiredTaskStatus = TaskStatus.Completed;

        var taskStatuses = new[]
        {
            TaskStatus.Completed,
            TaskStatus.Completed,
            TaskStatus.InProgress,
            TaskStatus.NotStarted,
            TaskStatus.CompletedWithMissedDeadline
        };

        var userTaskStatusFaker = new AutoFaker<Domain.Entities.UserTaskStatus>();
        var userTaskStatuses = userTaskStatusFaker
            .RuleFor(x => x.UserId, userId)
            .RuleFor(x => x.Task, (_, _) => null)
            .RuleFor(x => x.TaskStatus, f => taskStatuses[f.IndexFaker])
            .Generate(countToGenerate);

        var taskFaker = new AutoFaker<Domain.Entities.Task>();
        var tasks = taskFaker
            .RuleFor(x => x.UserTaskStatuses, 
                f => new List<Domain.Entities.UserTaskStatus> { userTaskStatuses[f.IndexFaker] })
            .Generate(countToGenerate);

        for (var i = 0; i < countToGenerate; i++)
        {
            userTaskStatuses[i].Task = tasks[i];
        }

        var userTaskStatusDtos = userTaskStatuses.Take(2).Select(_mapper.Map);
        
        var query = new GetUserTaskStatusesFromChatForUserQuery { ChatId = chatId, UserId = userId, Status = desiredTaskStatus};
        var handler = new GetUserTaskStatusesFromChatForUserQueryHandler(_unitOfWork, _mapper);
        _unitOfWork.TaskRepository.GetTasksFromChatAsync(Arg.Is<long>(x => x == chatId)).Returns(tasks);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(userTaskStatusDtos);
    }
}