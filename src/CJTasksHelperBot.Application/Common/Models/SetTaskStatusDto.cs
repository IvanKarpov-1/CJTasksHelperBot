using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Application.Common.Models;

public class SetTaskStatusDto
{
    public long UserId { get; init; }
    public string? PartialTaskId { get; init; }
    public string? PartialTaskTitle { get; init; }
    public TaskStatus TaskStatus { get; init; }
}