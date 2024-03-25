using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.Task.Queries;
using CJTasksHelperBot.Domain.Enums;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Application.Tests.Unit.Task.Queries;

public class GetSoonExpiredTasksQueryHandlerTasks
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly MapperlyMapper _mapper;

    public GetSoonExpiredTasksQueryHandlerTasks()
    {
        _mapper = new MapperlyMapper();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenTasksWithSoonExpiredDeadlinesIsNotFound_ReturnEmptyLists()
    {
        // Arrange
        var tasks = new List<Domain.Entities.Task>();
        var query = new GetSoonExpiredTasksQuery();
        var handler = new GetSoonExpiredTasksQueryHandler(_unitOfWork, _mapper);
        _unitOfWork.TaskRepository.GetSoonExpiredTasksAsync().Returns(tasks);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Item1.Should().BeEmpty();
        result.Value.Item2.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenGetSoonExpiredTasks_SetAppropriateNotificationLevel()
    {
        // Arrange
        var deadlinesWithNotLevels = new List<(DateTime, NotificationLevel)>
        {
            (DateTime.UtcNow.AddDays(1), NotificationLevel.Never),
            (DateTime.UtcNow.AddDays(2), NotificationLevel.Day),
            (DateTime.UtcNow.AddDays(3), NotificationLevel.TwoDays),
            (DateTime.UtcNow.AddDays(8), NotificationLevel.Week),
        };

        var faker = new AutoFaker<Domain.Entities.Task>();
        faker.RuleFor(x => x.Deadline, f => deadlinesWithNotLevels[f.IndexFaker].Item1)
            .RuleFor(x => x.NotificationLevel, NotificationLevel.Week);

        var tasks = faker.Generate(deadlinesWithNotLevels.Count);
        var query = new GetSoonExpiredTasksQuery();
        var handler = new GetSoonExpiredTasksQueryHandler(_unitOfWork, _mapper);
        _unitOfWork.TaskRepository.GetSoonExpiredTasksAsync().Returns(tasks);

        // Act
        await handler.Handle(query, default);

        // Assert
        for (var i = 0; i < deadlinesWithNotLevels.Count; i++)
        {
            tasks[i].NotificationLevel.Should().Be(deadlinesWithNotLevels[i].Item2);
        }
    }

    private class Grouping<TKey, TElement> : List<TElement>, IGrouping<TKey, TElement>
    {
        public Grouping(TKey key, TElement element)
            : base(new List<TElement> { element }) => Key = key;

        public TKey Key { get; }
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenGetSoonExpiredTasks_ReturnTasksPerGroup()
    {
        // Arrange
        const int countOfTasks = 5;
        var faker = new AutoFaker();
        var taskIds = faker.Generate<Guid>(countOfTasks);

        var userChats = faker.Generate<Domain.Entities.UserChat>(countOfTasks);
        foreach (var userChat in userChats)
        {
            userChat.UserId = userChat.User!.Id;
            userChat.ChatId = userChat.Chat!.Id;
        }

        userChats[0].Chat!.Id = userChats[0].User!.Id;
        userChats[0].ChatId = userChats[0].UserId;

        var taskFaker = new AutoFaker<Domain.Entities.Task>();
        var tasks = taskFaker
            .RuleFor(x => x.Id, f => taskIds[f.IndexFaker])
            .RuleFor(x => x.UserChat, f => userChats[f.IndexFaker])
            .Generate(countOfTasks);

        var getTasksDto = tasks.Select(_mapper.Map).ToList();

        var tasksPerGroup = new List<IGrouping<long, GetTaskDto>>();
        for (var i = 1; i < countOfTasks; i++)
        {
            tasksPerGroup.Add(new Grouping<long, GetTaskDto>(userChats[i].ChatId, getTasksDto[i]));
        }

        _unitOfWork.TaskRepository.GetSoonExpiredTasksAsync().Returns(tasks);

        var query = new GetSoonExpiredTasksQuery();
        var handler = new GetSoonExpiredTasksQueryHandler(_unitOfWork, _mapper);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Item1.Should().BeEquivalentTo(tasksPerGroup);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenGetSoonExpiredTasks_ReturnTasksPerUser()
    {
        // Arrange
        const int countOfTasks = 5;
        var faker = new AutoFaker();

        var userTaskStatuses = faker.Generate<Domain.Entities.UserTaskStatus>(countOfTasks);

        var taskFaker = new AutoFaker<Domain.Entities.Task>();
        var tasks = taskFaker
            .RuleFor(x => x.UserTaskStatuses, f =>
                new List<Domain.Entities.UserTaskStatus> { userTaskStatuses[f.IndexFaker] })
            .Generate(countOfTasks);

        for (var i = 0; i < countOfTasks; i++)
        {
            userTaskStatuses[i].Task = tasks[i];
        }

        var allowedTaskStatuses = new[]
        {
            TaskStatus.DeadlineMissed,
            TaskStatus.Completed,
            TaskStatus.CompletedWithMissedDeadline
        };

        var tasksPerUser = userTaskStatuses
            .Where(userTaskStatus => !allowedTaskStatuses.Contains(userTaskStatus.TaskStatus)).ToDictionary(
                userTaskStatus => userTaskStatus.UserId,
                userTaskStatus => new Dictionary<string, List<GetTaskDto>>
                    { { userTaskStatus.Task!.UserChat!.Chat!.Title!, [_mapper.Map(userTaskStatus.Task!)] } });

        _unitOfWork.TaskRepository.GetSoonExpiredTasksAsync().Returns(tasks);

        var query = new GetSoonExpiredTasksQuery();
        var handler = new GetSoonExpiredTasksQueryHandler(_unitOfWork, _mapper);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Item2.Should().BeEquivalentTo(tasksPerUser);
    }
}