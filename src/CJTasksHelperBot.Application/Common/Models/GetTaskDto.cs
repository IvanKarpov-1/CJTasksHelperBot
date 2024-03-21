using CJTasksHelperBot.Domain.Enums;

namespace CJTasksHelperBot.Application.Common.Models;

public class GetTaskDto
{
	public Guid Id { get; set; }
	public string? Title { get; set; }
	public string? Description { get; set; }
	public DateTime Deadline { get; set; }
	public DateTime CompletedAt { get; set; }
	public NotificationLevel NotificationLevel { get; set; }
}