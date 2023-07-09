using CJTasksHelperBot.Infrastructure.Common.Enums;

namespace CJTasksHelperBot.Infrastructure.Common;

public class StateObject
{
	public string? CallingCommand { get; set; }
	public string? CurrentStep { get; set; } = CommandStep.Initial.DisplayName;
	public Dictionary<string, object> Values { get; set; } = new();
}