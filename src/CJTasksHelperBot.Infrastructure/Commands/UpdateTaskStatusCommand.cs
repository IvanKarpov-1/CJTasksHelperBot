using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Infrastructure.Common;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Extensions;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Resources;
using MediatR;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Infrastructure.Commands;

public class UpdateTaskStatusCommand : ICommand
{
    private readonly ITelegramBotClient _botClient;
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly ICacheService _cacheService;

    public UpdateTaskStatusCommand(ITelegramBotClient botClient, IMediator mediator,
        IStringLocalizer<Messages> localizer, ICacheService cacheService)
    {
        _botClient = botClient;
        _mediator = mediator;
        _localizer = localizer;
        _cacheService = cacheService;
    }

    public CommandType CommandType => CommandType.UpdateTaskStatus;
    public bool IsAllowCommandLineArguments => true;

    public async Task ExecuteAsync(UserDto userDto, ChatDto chatDto, CancellationToken cancellationToken)
    {
        if (userDto.Id != chatDto.Id)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatDto.Id,
                text: _localizer["only_in_private"],
                cancellationToken: cancellationToken);
        }

        var stateObject = new StateObject
        {
            CallingCommand = CommandType.DisplayName,
            CurrentStep = CommandStep.WritingPartialTaskId.DisplayName
        };

        _cacheService.Add(userDto.Id, chatDto.Id, stateObject);

        await _botClient.SendTextMessageAsync(
            chatId: chatDto.Id,
            text: $"{_localizer["to_int_enter"]}\n\n{_localizer["enter_partial_task_id"]}",
            cancellationToken: cancellationToken);
    }

    public async Task ExecuteWithCommandLineArguments(UserDto userDto, ChatDto chatDto,
        Dictionary<string, string> arguments,
        CancellationToken cancellationToken)
    {
        if (userDto.Id != chatDto.Id)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatDto.Id,
                text: _localizer["only_in_private"],
                cancellationToken: cancellationToken);
        }

        var id = arguments.GetArgument(CommandLineArgument.PartialTaskId);
        var title = arguments.GetArgument(CommandLineArgument.PartialTaskTitle);

        if (id == string.Empty)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatDto.Id,
                text: $"{_localizer["need_enter_partial_task_id"]} `{CommandLineArgument.PartialTaskId.DisplayName}`",
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);

            return;
        }

        if (title == string.Empty)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatDto.Id,
                text: $"{_localizer["need_enter_task_name"]} `{CommandLineArgument.PartialTaskTitle.DisplayName}`",
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);

            return;
        }

        TaskStatus taskStatus;

        try
        {
            var statusId = arguments.GetArgument(CommandLineArgument.NewTaskStatus);
            taskStatus = (TaskStatus)TaskStatusCustomEnum.FromValue(int.Parse(statusId)).Id;

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

        var setTaskStatusDto = new SetTaskStatusDto
        {
            UserId = userDto.Id,
            PartialTaskId = id,
            PartialTaskTitle = title,
            TaskStatus = taskStatus,
        };

        var result =
            await _mediator.Send(
                new Application.UserTaskStatus.Commands.UpdateTaskStatusCommand { SetTaskStatusDto = setTaskStatusDto },
                cancellationToken);

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