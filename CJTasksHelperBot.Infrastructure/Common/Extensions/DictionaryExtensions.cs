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
				if (dictionary.ContainsKey(keys[0])) return dictionary[keys[0]];
				if (dictionary.ContainsKey(keys[1])) return dictionary[keys[1]];
				break;
			}
		}

		return string.Empty;
	}
}