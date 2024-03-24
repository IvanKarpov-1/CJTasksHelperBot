namespace CJTasksHelperBot.Domain.Entities;

public class User
{
	public long Id { get; set; }
	public string? UserName { get; set; }
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
	public ICollection<Chat> Chats { get; init; } = new List<Chat>();
	public ICollection<UserTaskStatus> UserTaskStatuses { get; init; } = new List<UserTaskStatus>();
	public ICollection<Homework> Homeworks { get; init; } = new List<Homework>();
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}