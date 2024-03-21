using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Application.Common.Models;

public class SetTaskStatusDto
{
    public long UserId { get; set; }
    public string? PartialTaskId { get; set; }
    public string? PartialTaskTitle { get; set; }
    public TaskStatus TaskStatus { get; set; }
}