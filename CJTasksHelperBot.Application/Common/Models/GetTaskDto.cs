using CJTasksHelperBot.Domain.Enums;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Application.Common.Models;

public class GetTaskDto
{
	public string? Title { get; set; }
	public string? Description { get; set; }
	// public TaskStatus Status { get; set; }
	public DateTime Deadline { get; set; }
	public DateTime CompletedAt { get; set; }
	public NotificationLevel NotificationLevel { get; set; }
}