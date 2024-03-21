using CJTasksHelperBot.Infrastructure.Common.Enums;

namespace CJTasksHelperBot.Infrastructure.Common.Extensions;

public static class DictionaryExtensions
{
	public static string GetArgument(this Dictionary<string, string> dictionary, CommandLineArgument parameter)
	{
		var key = parameter.DisplayName;
		var isContainsVerticalRisk = key.Contains('|');
		switch (isContainsVerticalRisk)
		{
			case false when dictionary.ContainsKey(key):
				return dictionary[key];
			case true:
			{
				var keys = key.Split('|');
				foreach (var s in keys)
				{
					if (dictionary.ContainsKey(s)) return dictionary[s];
				}
				break;
			}
		}

		return string.Empty;
	}

	public static bool ContainsKey(this Dictionary<string, string> dictionary, CommandLineArgument parameter)
	{
		var key = parameter.DisplayName;
		var isContainsVerticalRisk = key.Contains('|');
		switch (isContainsVerticalRisk)
		{
			case false when dictionary.ContainsKey(key):
				return true;
			case true:
			{
				var keys = key.Split('|');
				if (keys.Any(dictionary.ContainsKey))
				{
					return true;
				}
				break;
			}
		}

		return false;
	}
}