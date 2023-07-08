namespace CJTasksHelperBot.Domain.Enums;

public record TaskStatus(int Id, string DisplayName) : Enumeration<TaskStatus>(Id, DisplayName)
{
	public static readonly TaskStatus NotStarted = new(1, "Не розпочато");
	public static readonly TaskStatus InProgress = new(2, "В процесі");
	public static readonly TaskStatus AlmostDone = new(3, "Майже готово");
	public static readonly TaskStatus Completed = new(4, "Виконано");
	public static readonly TaskStatus DeadlineMissed = new(5, "Дедлайно пропущено");
	public static readonly TaskStatus CompletedWithMissedDeadline = new(6, "Виконано з пропущенним дедлайном");
}