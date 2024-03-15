using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Application.UserTaskStatus.Queries;

public class GetUserTaskStatusesFromChatForUserQuery : IRequest<Result<List<UserTaskStatusDto>>>
{
    public long? UserId { get; init; }
    public long? ChatId { get; init; }
    public TaskStatus? Status { get; init; }
}

public class GetUserTaskStatusesFromChatForUserQueryHandler : IRequestHandler<GetUserTaskStatusesFromChatForUserQuery,
    Result<List<UserTaskStatusDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;

    public GetUserTaskStatusesFromChatForUserQueryHandler(IUnitOfWork unitOfWork, MapperlyMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<UserTaskStatusDto>>> Handle(GetUserTaskStatusesFromChatForUserQuery request, CancellationToken cancellationToken)
    {
        if (request.ChatId == null) return Result<List<UserTaskStatusDto>>.Failure(["ChatId must not be null"]);
        
        var tasks = await _unitOfWork.TaskRepository.GetTasksFromChatAsync((long)request.ChatId);
        
        if (request.UserId != null)
        {
            foreach (var task in tasks.Where(task => task.UserTaskStatuses.All(x => x.UserId != request.UserId)))
            {
                task.UserTaskStatuses.Add(new Domain.Entities.UserTaskStatus
                {
                    Task = task,
                    UserId = (long)request.UserId!,
                });
            }
        }
        
        await _unitOfWork.CommitAsync();

        var userTaskStatuses = tasks
            .SelectMany(task =>
                task.UserTaskStatuses.Where(x =>
                    (request.Status == null || x.TaskStatus.CompareTo(request.Status) == 0) &&
                    (request.UserId == null || x.UserId == request.UserId)))
            .Select(_mapper.Map).ToList();
        
        return Result<List<UserTaskStatusDto>>.Success(userTaskStatuses);
    }
}