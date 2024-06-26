﻿using CJTasksHelperBot.Domain;

namespace CJTasksHelperBot.Infrastructure.Common.Enums;

public record CommandStep(int Id, string DisplayName) : Enumeration<CommandStep>(Id, DisplayName)
{
	public static readonly CommandStep Stop = new(0, "/stop");

	public static readonly CommandStep WritingTaskTitle = new(1, "WritingTaskTitle");
	public static readonly CommandStep WritingTaskDescription = new(2, "WritingTaskDescription");
	public static readonly CommandStep WritingTaskDeadline = new(3, "WritingTaskDeadline");

	public static readonly CommandStep WritingLanguageCode = new(4, "WritingLanguageCode");

	public static readonly CommandStep WritingPartialTaskId = new(5, "WritingPartialTaskId");
	public static readonly CommandStep WritingPartialTaskTitle = new(6, "WritingPartialTaskTitle");
	public static readonly CommandStep WritingNewTaskStatus = new(7, "WritingNewTaskStatus");
}