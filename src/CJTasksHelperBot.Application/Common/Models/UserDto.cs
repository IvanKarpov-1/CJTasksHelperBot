using CJTasksHelperBot.Domain.Enums;

namespace CJTasksHelperBot.Application.Common.Models;

public class UserDto
{
	public long Id { get; set; }
	public string? UserName { get; set; }
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
}