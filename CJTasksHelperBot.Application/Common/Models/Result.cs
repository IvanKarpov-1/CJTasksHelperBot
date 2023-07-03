namespace CJTasksHelperBot.Application.Common.Models;

public class Result<T>
{
	public bool IsSuccess { get; set; }
	public T? Value { get; set; }
	public string[]? Errors { get; set; }

	public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
	public static Result<T> Failure(string[] errors) => new() { IsSuccess = false, Errors = errors };
}