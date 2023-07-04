using CJTasksHelperBot.Domain.Common;

namespace CJTasksHelperBot.Domain.Entities;

public class Chat : BaseEntity
{
	public long TelegramId { get; set; }
	public string? Title { get; set; }
	public ICollection<User> Users { get; set; } = new List<User>();
}