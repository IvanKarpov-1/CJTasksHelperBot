using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Domain.Entities;
using MediatR;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Application.Task.Commands;

public class SetDeadlineMissedStatusCommand : IRequest<Result<Unit>>
{
}

public class SetDeadlineMissedStatusCommandHandler : IRequestHandler<SetDeadlineMissedStatusCommand, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetDeadlineMissedStatusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(SetDeadlineMissedStatusCommand request, CancellationToken cancellationToken)
    {
        var taskStatuses = await _unitOfWork
            .GetRepository<UserTaskStatus>()
            .GetWhereAsync(x => x.Task != null &&
                                x.Task.Deadline < DateTime.UtcNow &&
                                new[]
                                {
                                    TaskStatus.AlmostDone,
                                    TaskStatus.InProgress,
                                    TaskStatus.NotStarted
                                }.Contains(x.TaskStatus));

        foreach (var taskStatus in taskStatuses)
        {
            taskStatus.TaskStatus = TaskStatus.DeadlineMissed;
        }

        await _unitOfWork.CommitAsync();

        return Result<Unit>.Success(Unit.Value);
    }
}