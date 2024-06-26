﻿using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Domain.Entities;

public class Homework
{
	public Guid Id { get; set; }
	public string? Subject { get; set; }
	public string? Task { get; set; }
	public TaskStatus Status { get; set; } = TaskStatus.NotStarted;
	public DateTime Deadline { get; set; }
	public DateTime CompletedAd { get; init; }
	public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
	public UserChat? UserChat { get; set; }
}