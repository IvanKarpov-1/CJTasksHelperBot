namespace CJTasksHelperBot.Application.Common.Models;

public class GetTaskDto
{
	public string? Title { get; set; }
	public string? Description { get; set; }
	public string? Status { get; set; }
	public DateTime Deadline { get; set; }
	public DateTime CompletedAt { get; set; }
}