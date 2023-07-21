using CJTasksHelperBot.Domain;

namespace CJTasksHelperBot.Infrastructure.Common.Enums;

public record CallbackQueriesDataKey(int Id, string DisplayName) : Enumeration<CallbackQueriesDataKey>(Id, DisplayName)
{
	public static readonly CallbackQueriesDataKey TelegramId = new(1, "TelegramId");
	public static readonly CallbackQueriesDataKey IsNeedDrawTable = new(2, "IsNeedDrawTable");
	public static readonly CallbackQueriesDataKey Toggle = new(3, "Toggle");
}