using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Application.Common.Models;

public class UserTaskStatusDto
{
    public long UserId { get; set; }
    public UserDto? User { get; set; }
    public GetTaskDto? Task { get; set; }
    public TaskStatus TaskStatus { get; set; }
}