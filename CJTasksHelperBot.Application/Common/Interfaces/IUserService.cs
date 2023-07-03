namespace CJTasksHelperBot.Application.Common.Interfaces;

public interface IUserService
{
	Task<bool> CheckUserExistenceAsync(long userId);
}