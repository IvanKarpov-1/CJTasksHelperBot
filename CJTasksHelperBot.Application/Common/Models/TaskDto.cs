namespace CJTasksHelperBot.Application.Common.Models;

public class TaskDto
{
	public string? Title { get; set; }
	public string? Description { get; set; }
	public int StatusId { get; set; }
	public DateTime Deadline { get; set; }
	public DateTime CompletedAd { get; set; }
	public DateTime CreatedAt { get; set; }
}