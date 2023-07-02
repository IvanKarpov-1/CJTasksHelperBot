using CJTasksHelperBot.Domain.Common;

namespace CJTasksHelperBot.Domain.Entities;

public class User : BaseEntity
{
	public long TelegramId { get; set; }
	public string? UserName { get; set; }
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
	public ICollection<UserChat> UserChats { get; set; } = new List<UserChat>();
	public ICollection<Task> Tasks { get; set; } = new List<Task>();
	public ICollection<Homework> Homeworks { get; set; } = new List<Homework>();
}