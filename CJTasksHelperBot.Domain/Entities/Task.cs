using CJTasksHelperBot.Domain.Common;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Domain.Entities;

public class Task/* : BaseEntity*/
{
	public Guid Id { get; set; }
	public string? Title { get; set; }
	public string? Description { get; set; }
	public TaskStatus Status { get; set; } = TaskStatus.NotStarted;
	public DateTime Deadline { get; set; }
	public DateTime CompletedAd { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}