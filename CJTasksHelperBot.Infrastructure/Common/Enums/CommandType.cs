using CJTasksHelperBot.Domain;

namespace CJTasksHelperBot.Infrastructure.Common.Enums;

public record CommandType(int Id, string DisplayName) : Enumeration<CommandType>(Id, DisplayName)
{
	public static readonly CommandType Help = new(0, "/help");
	public static readonly CommandType Start = new(1, "/start");
    public static readonly CommandType GetCurrentUser = new(2, "/get_current_user");
    public static readonly CommandType AddTask = new(3, "/add_task");
    public static readonly CommandType GetTasks = new(4, "/get_tasks");
    public static readonly CommandType ChangeLanguage = new(5, "/chlang");
}