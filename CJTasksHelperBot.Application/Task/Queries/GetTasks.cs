using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;
using CJTasksHelperBot.Domain.Entities;
using TaskStatusCustomEnum = CJTasksHelperBot.Domain.Enums.TaskStatusCustomEnum;

namespace CJTasksHelperBot.Application.Task.Queries;

public class GetTasksQuery : IRequest<Result<List<GetTaskDto>>>
{
    public long? UserId { get; set; }
    public long? ChatId { get; set; }
    public TaskStatusCustomEnum? Status { get; set; }
}

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, Result<List<GetTaskDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;

    public GetTasksQueryHandler(IUnitOfWork unitOfWork, MapperlyMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<GetTaskDto>>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        if (request.ChatId == null) return Result<List<GetTaskDto>>.Failure(["ChatId must not be null"]);
        
        var tasks = await _unitOfWork.TaskRepository.GetTasksFromChatAsync((long)request.ChatId!);

        foreach (var task in tasks.Where(task => task.UserTaskStatuses.All(x => x.UserId != request.UserId)))
        {
            task.UserTaskStatuses.Add(new UserTaskStatus
            {
                Task = task,
                UserId = (long)request.UserId!,
            });
        }

        await _unitOfWork.CommitAsync();

        var tasksDto = tasks.Where((Func<Domain.Entities.Task, bool>)Predicate2).Select(_mapper.Map).ToList();

        return Result<List<GetTaskDto>>.Success(tasksDto);

        bool Predicate2(Domain.Entities.Task task) =>
            task.UserTaskStatuses.Any(x =>
                (request.Status == null || x.TaskStatus.CompareTo(request.Status.Id) == 0) &&
                x.UserId == task.UserChat!.UserId);
    }
}