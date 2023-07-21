using CJTasksHelperBot.Domain;

namespace CJTasksHelperBot.Infrastructure.Common.Enums;

public record CommandHelp(int Id, string DisplayName) : Enumeration<CommandHelp>(Id, DisplayName)
{
	public static readonly CommandHelp AddTaskCommandHelp = new(CommandType.AddTask.Id, "Test");
	public static readonly CommandHelp GetTasksCommandHelp = new(CommandType.GetTasks.Id, "Test");
}