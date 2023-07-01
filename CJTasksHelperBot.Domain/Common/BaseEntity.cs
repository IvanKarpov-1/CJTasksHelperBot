using System.ComponentModel.DataAnnotations;

namespace CJTasksHelperBot.Domain.Common;

public class BaseEntity
{
	[Key]
	public Guid Id { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}