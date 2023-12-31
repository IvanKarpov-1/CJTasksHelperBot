﻿using CJTasksHelperBot.Domain;

namespace CJTasksHelperBot.Infrastructure.Common.Enums;

public record CommandLineArgument(int Id, string DisplayName) : Enumeration<CommandLineArgument>(Id, DisplayName)
{
	public static readonly CommandLineArgument Help = new(0, "-h|--help");

	public static readonly CommandLineArgument Title = new(1, "-t|--title");
	public static readonly CommandLineArgument Description = new(2, "--des|--description");
	public static readonly CommandLineArgument Deadline = new(3, "--dl|--deadline");
	public static readonly CommandLineArgument DrawTable = new(4, "--dt|--drawtable");
}