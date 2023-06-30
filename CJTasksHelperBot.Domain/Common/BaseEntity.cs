using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CJTasksHelperBot.Domain.Common;

public class BaseEntity
{
	[Key]
	public Guid Id { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}