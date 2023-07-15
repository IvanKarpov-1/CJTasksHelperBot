using CJTasksHelperBot.Domain;

namespace CJTasksHelperBot.Infrastructure.Common.Extensions;

public static class EnumerationExtensions
{
	public static string GetDisplayNameWithEscapingCharacter<T>(this Enumeration<T> enumeration) where T : Enumeration<T>
	{
		return enumeration.DisplayName.Replace("|", "\\|").Replace("_", "\\_");
	}
}