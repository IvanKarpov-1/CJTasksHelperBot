using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;

namespace CJTasksHelperBot.Infrastructure.Services;

public class StepService : IStepService
{
	private readonly IEnumerable<IStep> _steps;
	private readonly ICommandStateService _commandStateService;

	public StepService(IEnumerable<IStep> steps, ICommandStateService commandStateService)
	{
		_steps = steps;
		_commandStateService = commandStateService;
	}

	public Task HandleTextCommandStepAsync(UserDto userDto, ChatDto chatDto, string text, CancellationToken cancellationToken)
	{
		var stateObject = _commandStateService.GetStateObject<StateObject>(userDto.Id, chatDto.Id);
		var commandStep = GetStep(stateObject?.CurrentStep!);
		commandStep?.PerformStepAsync(userDto, chatDto, text, cancellationToken);
		return Task.CompletedTask;
	}

	private IStep? GetStep(string stepName)
	{
		return _steps.FirstOrDefault(x => x.CommandStep == CommandStep.FromDisplayName(stepName), null);
	}
}