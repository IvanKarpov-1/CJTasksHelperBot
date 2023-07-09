﻿using CJTasksHelperBot.Domain;

namespace CJTasksHelperBot.Infrastructure.Common.Enums;

public record CommandStep(int Id, string DisplayName) : Enumeration<CommandStep>(Id, DisplayName)
{
	public static readonly CommandStep Initial = new(0, "Initial");

	public static readonly CommandStep WritingTaskTitle = new(1, "WritingTaskTitle");
	public static readonly CommandStep WritingTaskDescription = new(2, "WritingTaskDescription");
	public static readonly CommandStep WritingTaskDeadline = new(3, "WritingTaskDeadline");
}