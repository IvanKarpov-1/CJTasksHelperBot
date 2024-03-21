using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Enums;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces;

public interface IStep
{
	CommandStep CommandStep { get; }
	Task PerformStepAsync(UserDto userDto, ChatDto chatDto, string text, CancellationToken cancellationToken);
}