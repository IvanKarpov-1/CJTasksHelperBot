using CJTasksHelperBot.Domain;

namespace CJTasksHelperBot.Infrastructure.Common.Enums;

public record CommandType(int Id, string DisplayName) : Enumeration<CommandType>(Id, DisplayName)
{
    public static readonly CommandType StartCommand = new(1, "/start");
    public static readonly CommandType GetCurrentUserCommand = new(2, "/get_current_user");
    public static readonly CommandType AddTask = new(3, "/add_task");
}