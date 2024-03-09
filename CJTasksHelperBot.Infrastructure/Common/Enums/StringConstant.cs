using CJTasksHelperBot.Domain;

namespace CJTasksHelperBot.Infrastructure.Common.Enums;

public record StringConstant(int Id, string DisplayName) : Enumeration<StringConstant>(Id, DisplayName)
{
	public static readonly StringConstant ToggleOn = new(2, "✅");
	public static readonly StringConstant ToggleOff = new(3, "❌");
}