using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Domain.Entities;

public class UserTaskStatus
{
    public long UserId { get; set; }
    public User? User { get; set; }
    public Guid TaskId { get; set; }
    public Task? Task { get; set; }
    public TaskStatus TaskStatus { get; set; } = TaskStatus.NotStarted;
}