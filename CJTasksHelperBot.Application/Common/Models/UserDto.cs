namespace CJTasksHelperBot.Application.Common.Models;

public class UserDto
{
	public long TelegramId { get; set; }
	public string? UserName { get; set; }
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
}