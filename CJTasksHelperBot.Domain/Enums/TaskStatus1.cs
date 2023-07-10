namespace CJTasksHelperBot.Domain.Enums;

public record TaskStatus1(int Id, string DisplayName) : Enumeration<TaskStatus1>(Id, DisplayName)
{
	public static readonly TaskStatus1 NotStarted = new(1, "Не розпочато");
	public static readonly TaskStatus1 InProgress = new(2, "В процесі");
	public static readonly TaskStatus1 AlmostDone = new(3, "Майже готово");
	public static readonly TaskStatus1 Completed = new(4, "Виконано");
	public static readonly TaskStatus1 DeadlineMissed = new(5, "Дедлайно пропущено");
	public static readonly TaskStatus1 CompletedWithMissedDeadline = new(6, "Виконано з пропущенним дедлайном");
}