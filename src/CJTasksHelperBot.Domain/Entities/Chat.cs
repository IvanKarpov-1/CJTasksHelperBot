using CJTasksHelperBot.Domain.Enums;

namespace CJTasksHelperBot.Domain.Entities;

public class Chat
{
	public long Id { get; set; }
	public string? Title { get; set; }
	public ICollection<User> Users { get; init; } = new List<User>();
	public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
	public LanguageCode LanguageCode { get; set; } = LanguageCode.Uk;
}