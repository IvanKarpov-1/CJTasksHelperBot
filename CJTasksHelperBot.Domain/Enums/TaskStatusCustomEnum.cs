namespace CJTasksHelperBot.Domain.Enums;

public record TaskStatusCustomEnum(int Id, string DisplayName) : Enumeration<TaskStatusCustomEnum>(Id, DisplayName)
{
	public static readonly TaskStatusCustomEnum NotStarted = new((int)TaskStatus.NotStarted, "TaskNotStarted");
	public static readonly TaskStatusCustomEnum InProgress = new((int)TaskStatus.InProgress, "TaskInProgress");
	public static readonly TaskStatusCustomEnum AlmostDone = new((int)TaskStatus.AlmostDone, "TaskAlmostDone");
	public static readonly TaskStatusCustomEnum Completed = new((int)TaskStatus.Completed, "TaskCompleted");
	public static readonly TaskStatusCustomEnum DeadlineMissed = new((int)TaskStatus.DeadlineMissed, "TaskDeadlineMissed");
	public static readonly TaskStatusCustomEnum CompletedWithMissedDeadline = new((int)TaskStatus.CompletedWithMissedDeadline, "TaskCompletedWithMissedDeadline");
}