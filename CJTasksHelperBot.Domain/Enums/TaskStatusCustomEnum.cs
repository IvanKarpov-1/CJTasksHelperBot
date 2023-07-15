namespace CJTasksHelperBot.Domain.Enums;

public record TaskStatusCustomEnum(int Id, string DisplayName) : Enumeration<TaskStatusCustomEnum>(Id, DisplayName)
{
	public static readonly TaskStatusCustomEnum NotStarted = new((int)TaskStatus.NotStarted, "Не розпочато");
	public static readonly TaskStatusCustomEnum InProgress = new((int)TaskStatus.InProgress, "В процесі");
	public static readonly TaskStatusCustomEnum AlmostDone = new((int)TaskStatus.AlmostDone, "Майже готово");
	public static readonly TaskStatusCustomEnum Completed = new((int)TaskStatus.Completed, "Виконано");
	public static readonly TaskStatusCustomEnum DeadlineMissed = new((int)TaskStatus.DeadlineMissed, "Дедлайно пропущено");
	public static readonly TaskStatusCustomEnum CompletedWithMissedDeadline = new((int)TaskStatus.CompletedWithMissedDeadline, "Виконано з пропущенним дедлайном");
}