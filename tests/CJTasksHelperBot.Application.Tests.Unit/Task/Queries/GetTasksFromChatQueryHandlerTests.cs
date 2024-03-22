using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Task.Queries;

namespace CJTasksHelperBot.Application.Tests.Unit.Task.Queries;

public class GetTasksFromChatQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly MapperlyMapper _mapper;

    public GetTasksFromChatQueryHandlerTests()
    {
        _mapper = new MapperlyMapper();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnFailureResult_WhenChatIdIsNull()
    {
        // Arrange
        var query = new GetTasksFromChatQuery();
        var handler = new GetTasksFromChatQueryHandler(_unitOfWork, _mapper);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be("ChatId must not be null");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnSuccessfulResult_WhenChatIdIsNotNull()
    {
        // Arrange
        var faker = new AutoFaker();
        var tasks = faker.Generate<Domain.Entities.Task>(5);
        var taskDtos = tasks.Select(_mapper.Map);
        var query = new GetTasksFromChatQuery{ ChatId = 1 };
        var handler = new GetTasksFromChatQueryHandler(_unitOfWork, _mapper);
        _unitOfWork.TaskRepository.GetTasksFromChatAsync(Arg.Any<long>()).Returns(tasks);
        
        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(taskDtos);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_AddUserTaskStatusToTask_WhenThereIsNoUserTaskStatusWithSuchUserId()
    {
        // Arrange
        long? userId = 1;

        var userTaskStatuses1 = new List<List<Domain.Entities.UserTaskStatus>>
        {
            new(),
            new(),
            new(),
            new(),
            new(),
        };
        
        var taskFaker = new AutoFaker<Domain.Entities.Task>();
        var tasks = taskFaker
            .RuleFor(x => x.UserTaskStatuses, f => userTaskStatuses1[f.IndexFaker])
            .Generate(5);
        
        var query = new GetTasksFromChatQuery{ UserId = userId, ChatId = 2 };
        var handler = new GetTasksFromChatQueryHandler(_unitOfWork, _mapper);
        _unitOfWork.TaskRepository.GetTasksFromChatAsync(Arg.Any<long>()).Returns(tasks);
        
        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        foreach (var task in tasks)
        {
            var userTaskStatuses = task.UserTaskStatuses.ToList();
            userTaskStatuses[0].Task.Should().BeEquivalentTo(task);
            userTaskStatuses[0].UserId.Should().Be(userId);
        }
    }
}