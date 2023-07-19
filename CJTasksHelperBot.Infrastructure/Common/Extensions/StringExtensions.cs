namespace CJTasksHelperBot.Infrastructure.Common.Extensions;

public static class StringExtensions
{
	public static string EscapeCharacters(this string value)
	{
		return value
			.Replace("_", "\\_")
			.Replace("*", "\\*")
			.Replace("[", "\\[")
			.Replace("]", "\\]")
			.Replace("(", "\\(")
			.Replace(")", "\\)")
			.Replace("~", "\\~")
			.Replace("`", "\\`")
			.Replace(">", "\\>")
			.Replace("#", "\\#")
			.Replace("+", "\\+")
			.Replace("=", "\\=")
			.Replace("-", "\\-")
			.Replace("|", "\\|")
			.Replace("{", "\\{")
			.Replace("}", "\\}")
			.Replace(".", "\\.")
			.Replace("!", "\\!");
	}
}