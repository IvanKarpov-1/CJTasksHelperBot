using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.UserTaskStatus.Commands;
using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Infrastructure.Common;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Resources;
using MediatR;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Infrastructure.CommandSteps.UpdateTaskStatus;

public class WritingNewTaskStatusStep : IStep
{
    private readonly ITelegramBotClient _botClient;
    private readonly ICacheService _cacheService;
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<Messages> _localizer;

    public WritingNewTaskStatusStep(ITelegramBotClient botClient, ICacheService cacheService, IMediator mediator, IStringLocalizer<Messages> localizer)
    {
        _botClient = botClient;
        _cacheService = cacheService;
        _mediator = mediator;
        _localizer = localizer;
    }

    public CommandStep CommandStep => CommandStep.WritingNewTaskStatus;
    
    public async Task PerformStepAsync(UserDto userDto, ChatDto chatDto, string text, CancellationToken cancellationToken)
    {
        TaskStatus taskStatus;
        
        try
        {
            taskStatus = (TaskStatus)TaskStatusCustomEnum.FromValue(int.Parse(text)).Id;

            var allowedStatuses = new[]
            {
                TaskStatus.NotStarted,
                TaskStatus.InProgress,
                TaskStatus.AlmostDone,
                TaskStatus.Completed
            };

            if (allowedStatuses.Contains(taskStatus) == false) throw new InvalidOperationException();
        }
        catch (InvalidOperationException)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatDto.Id,
                text: _localizer["incorrect_task_status_try_again"],
                cancellationToken: cancellationToken);
            return;
        }
        
        var stateObject = _cacheService.Get<StateObject>(userDto.Id, chatDto.Id);

        _cacheService.Delete(userDto.Id, chatDto.Id);

        if (stateObject == null)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatDto.Id,
                text: _localizer["err_update_task_status_try_again"],
                cancellationToken: cancellationToken);
            return;
        }

        var setTaskStatusDto = new SetTaskStatusDto
        {
            UserId = userDto.Id,
            PartialTaskId = stateObject.Values[CommandStep.WritingPartialTaskId.DisplayName].ToString(),
            PartialTaskTitle = stateObject.Values[CommandStep.WritingPartialTaskTitle.DisplayName].ToString(),
            TaskStatus = taskStatus,
        };

        var result = await _mediator.Send(new UpdateTaskStatusCommand { SetTaskStatusDto = setTaskStatusDto }, cancellationToken);

        if (result.IsSuccess)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatDto.Id,
                text: _localizer["task_status_updated"],
                cancellationToken: cancellationToken);
        }
        else
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatDto.Id,
                text: _localizer["err_update_task_status_try_again"],
                cancellationToken: cancellationToken);
        }
    }
}