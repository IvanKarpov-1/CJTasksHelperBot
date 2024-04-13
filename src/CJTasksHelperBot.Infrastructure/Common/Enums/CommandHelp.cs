using CJTasksHelperBot.Domain;

namespace CJTasksHelperBot.Infrastructure.Common.Enums;

public record CommandHelp(int Id, string DisplayName) : Enumeration<CommandHelp>(Id, DisplayName)
{
	public static readonly CommandHelp AddTaskCommandHelp = new(CommandType.AddTask.Id, "HelpAddTask");
	public static readonly CommandHelp GetTasksCommandHelp = new(CommandType.GetTasks.Id, "HelpGetTasks");
	public static readonly CommandHelp ChangeLanguageCommandHelp = new(CommandType.ChangeLanguage.Id, "HelpChangeLanguage");
	public static readonly CommandHelp UpdateTasksStatusCommandHelp = new(CommandType.UpdateTaskStatus.Id, "HelpUpdateTaskStatus");
}