using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Domain.Entities;

namespace CJTasksHelperBot.Infrastructure.Identity;

public class UserService : IUserService
{
	private readonly IUnitOfWork _unitOfWork;

	public UserService(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public async Task<bool> CheckUserExistenceAsync(long userId)
	{
		var user = await _unitOfWork.GetRepository<User>().FindAsync(x => x.TelegramId == userId);
		return user == null;
	}
}