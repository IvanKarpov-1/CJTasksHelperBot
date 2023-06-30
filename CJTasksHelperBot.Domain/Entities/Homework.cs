using CJTasksHelperBot.Domain.Common;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Domain.Entities;

public class Homework : BaseEntity
{
	public string? Subject { get; set; }
	public string? Task { get; set; }
	public TaskStatus Status { get; set; } = TaskStatus.NotStarted;
	public DateTime Deadline { get; set; }
	public DateTime CompletedAd { get; set; }
}