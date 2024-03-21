using CJTasksHelperBot.Domain.Enums;

namespace CJTasksHelperBot.Application.Common.Models;

public class UserChatDto
{
	public long UserId { get; set; }
	public UserDto? UserDto { get; set; }
	public long ChatId { get; set; }
	public ChatDto? ChatDto { get; set; }
}