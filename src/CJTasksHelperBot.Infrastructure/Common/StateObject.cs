namespace CJTasksHelperBot.Infrastructure.Common;

public class StateObject
{
	public string? CallingCommand { get; init; }
	public string? CurrentStep { get; set; }
	public Dictionary<string, object> Values { get; } = new();
}