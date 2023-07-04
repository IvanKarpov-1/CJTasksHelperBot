using CJTasksHelperBot.Infrastructure.CommandType.Enumeration;

namespace CJTasksHelperBot.Infrastructure.CommandType;

public record CommandType(int Id, string DisplayName) : Enumeration<CommandType>(Id, DisplayName)
{
	public static readonly CommandType StartCommand = new(1, "/start");
	public static readonly CommandType GetCurrentUserCommand = new(2, "/get_current_user");
}