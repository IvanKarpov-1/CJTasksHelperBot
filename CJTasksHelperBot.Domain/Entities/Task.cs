using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Domain.Extensions;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Domain.Entities;

public class Task
{
	public Guid Id { get; set; }
	public string? Title { get; set; }
	public string? Description { get; set; }
	public TaskStatus Status { get; set; } = TaskStatus.NotStarted;
	public DateTime Deadline { get; set; }
	public DateTime CompletedAt { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public UserChat? UserChat { get; set; }
	public NotificationLevel NotificationLevel { get; set; } = NotificationLevel.Week;
	
	public void SetNotificationLevel(NotificationLevel? notificationLevel = null)
	{
		if (notificationLevel == null)
		{
			NotificationLevel = NotificationLevel.GetNextValueUntilLastByDeclarationOrder();
		}
		else
		{
			NotificationLevel = (NotificationLevel)notificationLevel!;
		}
	}
}