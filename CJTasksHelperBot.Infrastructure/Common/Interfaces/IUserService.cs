using CJTasksHelperBot.Application.Common.Models;
using User = Telegram.Bot.Types.User;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces;

public interface IUserService
{
	public Task<UserDto?> FindUserByIdAsync(long id);
	public Task<UserDto> GetUserFromTelegramModelAsync(User user);
}