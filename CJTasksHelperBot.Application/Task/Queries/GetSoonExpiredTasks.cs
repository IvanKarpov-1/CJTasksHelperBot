using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Application.Task.Queries;

public class GetSoonExpiredTasksQuery : IRequest<Result<(List<IGrouping<long, GetTaskDto>>,
    Dictionary<long, Dictionary<string, List<GetTaskDto>>>)>>
{
}

public class GetSoonExpiredTasksQueryHandler : IRequestHandler<GetSoonExpiredTasksQuery, Result<(
    List<IGrouping<long, GetTaskDto>>,
    Dictionary<long, Dictionary<string, List<GetTaskDto>>>)>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;

    public GetSoonExpiredTasksQueryHandler(IUnitOfWork unitOfWork, MapperlyMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async
        Task<Result<(List<IGrouping<long, GetTaskDto>>,
            Dictionary<long, Dictionary<string, List<GetTaskDto>>>)>> Handle(GetSoonExpiredTasksQuery request,
            CancellationToken cancellationToken)
    {
        var tasks = await _unitOfWork.TaskRepository.GetSoonExpiredTasksAsync();

        var groupedTasks = tasks
            .GroupBy(x => new { x.UserChat!.User, x.UserChat.Chat })
            .ToList();

        var tasksPerGroup = groupedTasks
            .SelectMany(grouping => grouping
                .Select(task => new { grouping.Key.Chat!.Id, Task = _mapper.Map(task) }))
            .GroupBy(item => item.Id, item => item.Task)
            .ToList();

        var flattenTasks = new Dictionary<long, List<(Domain.Entities.Task, TaskStatus)>>();
        foreach (var grouping in groupedTasks)
        {
            foreach (var task in grouping)
            {
                foreach (var uts in task.UserTaskStatuses)
                {
                    if (flattenTasks.ContainsKey(uts.UserId) == false)
                        flattenTasks[uts.UserId] = [];
                    flattenTasks[uts.UserId].Add((task, uts.TaskStatus));
                }
            }
        }

        var tasksPerUser = new Dictionary<long, Dictionary<string, List<GetTaskDto>>>();
        foreach (var (userId, usersTasks) in flattenTasks)
        {
            var groupsTasks = new Dictionary<string, List<GetTaskDto>>();

            foreach (var tuple in usersTasks.Where(tuple => new[]
                     {
                         TaskStatus.DeadlineMissed,
                         TaskStatus.Completed,
                         TaskStatus.CompletedWithMissedDeadline
                     }.Contains(tuple.Item2) == false))
            {
                var key = tuple.Item1.UserChat!.Chat!.Title ?? string.Empty;
                if (groupsTasks.ContainsKey(key) == false)
                    groupsTasks[key] = [];
                groupsTasks[key].Add(_mapper.Map(tuple.Item1));
            }

            tasksPerUser[userId] = groupsTasks;
        }

        foreach (var task in tasks)
        {
            if (task.Deadline < DateTime.UtcNow.AddDays((int)NotificationLevel.Day))
                task.SetNotificationLevel(NotificationLevel.Never);
            else if (task.Deadline < DateTime.UtcNow.AddDays((int)NotificationLevel.TwoDays))
                task.SetNotificationLevel(NotificationLevel.Day);
            else
                task.SetNotificationLevel();
        }

        await _unitOfWork.CommitAsync();

        return Result<(List<IGrouping<long, GetTaskDto>>, Dictionary<long, Dictionary<string, List<GetTaskDto>>>)>
            .Success((tasksPerGroup, tasksPerUser));
    }
}