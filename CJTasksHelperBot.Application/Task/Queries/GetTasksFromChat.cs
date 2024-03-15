using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;

namespace CJTasksHelperBot.Application.Task.Queries;

public class GetTasksFromChatQuery : IRequest<Result<List<GetTaskDto>>>
{
    public long? UserId { get; init; }
    public long? ChatId { get; init; }
}

public class GetTasksFromChatQueryHandler : IRequestHandler<GetTasksFromChatQuery, Result<List<GetTaskDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;

    public GetTasksFromChatQueryHandler(IUnitOfWork unitOfWork, MapperlyMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<GetTaskDto>>> Handle(GetTasksFromChatQuery request, CancellationToken cancellationToken)
    {
        if (request.ChatId == null) return Result<List<GetTaskDto>>.Failure(["ChatId must not be null"]);
        
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

        var tasksDto = tasks.Select(_mapper.Map).ToList();

        return Result<List<GetTaskDto>>.Success(tasksDto);
    }
}