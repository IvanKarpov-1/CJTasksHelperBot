using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Application.Task.Commands;

public class SetDeadlineMissedStatusCommand : IRequest<Result<Unit>>;

public class SetDeadlineMissedStatusCommandHandler : IRequestHandler<SetDeadlineMissedStatusCommand, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetDeadlineMissedStatusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(SetDeadlineMissedStatusCommand request, CancellationToken cancellationToken)
    {
        var taskStatuses = await _unitOfWork.UserTaskStatusRepository.GetTasksWithMissedDeadlinesAsync();

        foreach (var taskStatus in taskStatuses)
        {
            taskStatus.TaskStatus = TaskStatus.DeadlineMissed;
        }

        await _unitOfWork.CommitAsync();

        return Result<Unit>.Success(Unit.Value);
    }
}