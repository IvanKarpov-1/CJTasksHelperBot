namespace CJTasksHelperBot.Application.Common.Models;

public class CreateTaskDto
{
	public string? Title { get; set; }
	public string? Description { get; set; }
	public DateTime Deadline { get; set; }
	public UserChatDto? UserChatDto { get; set; }
}