using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Application.UserTaskStatus.Commands;

public class UpdateTaskStatusCommand : IRequest<Result<Unit>>
{
    public SetTaskStatusDto? SetTaskStatusDto { get; init; } 
}

public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaskStatusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        if (request.SetTaskStatusDto == null) return Result<Unit>.Failure(["SetTaskStatusDto cannot be null"]);

        var userId = request.SetTaskStatusDto.UserId;
        var partialTaskId = request.SetTaskStatusDto.PartialTaskId;
        var partialTaskTitle = request.SetTaskStatusDto.PartialTaskTitle;
        var taskStatus = request.SetTaskStatusDto.TaskStatus;
        
        if (partialTaskId == null) return Result<Unit>.Failure(["Partial task ID cannot be null"]);
        if (partialTaskTitle == null) return Result<Unit>.Failure(["Partial task title cannot be null"]);
        
        var userTaskStatus = await _unitOfWork.UserTaskStatusRepository.GetUserTaskStatus(userId, partialTaskId, partialTaskTitle);

        if (userTaskStatus == null) return Result<Unit>.Failure(["Something went wrong whet trying to update Task Status"]);
        
        if (userTaskStatus!.TaskStatus == TaskStatus.DeadlineMissed &&
            taskStatus == TaskStatus.Completed)
        {
            userTaskStatus.TaskStatus = TaskStatus.CompletedWithMissedDeadline;
        }
        else
        {
            userTaskStatus.TaskStatus = taskStatus;
        }

        var result = await _unitOfWork.CommitAsync();
        
        return result > 0
            ? Result<Unit>.Success(Unit.Value)
            : Result<Unit>.Failure(["Something went wrong whet trying to update Task Status"]);
    }
}