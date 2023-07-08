using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Domain.Entities;

public class Homework
{
	public Guid Id { get; set; }
	public string? Subject { get; set; }
	public string? Task { get; set; }
	public TaskStatus Status { get; set; } = TaskStatus.NotStarted;
	public DateTime Deadline { get; set; }
	public DateTime CompletedAd { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}