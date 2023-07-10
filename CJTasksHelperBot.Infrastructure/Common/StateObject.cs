namespace CJTasksHelperBot.Infrastructure.Common;

public class StateObject
{
	public string? CallingCommand { get; set; }
	public string? CurrentStep { get; set; }
	public Dictionary<string, object> Values { get; set; } = new();
}