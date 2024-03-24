namespace CJTasksHelperBot.Application.Common.Models;

public class Result<T>
{
	public bool IsSuccess { get; private init; }
	public T? Value { get; private init; }
	public string[]? Errors { get; private init; }

	public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
	public static Result<T> Failure(string[] errors) => new() { IsSuccess = false, Errors = errors };
}