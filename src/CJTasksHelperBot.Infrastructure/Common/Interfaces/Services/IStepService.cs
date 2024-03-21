using CJTasksHelperBot.Application.Common.Models;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;

public interface IStepService
{
	Task HandleTextCommandStepAsync(UserDto userDto, ChatDto chatDto, string text, CancellationToken cancellationToken);
}