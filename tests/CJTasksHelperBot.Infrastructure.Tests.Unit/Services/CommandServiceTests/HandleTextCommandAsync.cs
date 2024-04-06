using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Services;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Tests.Unit.Services.CommandServiceTests;

public class HandleTextCommandAsync
{
    private readonly CommandService _commandService;
    private readonly IEnumerable<ICommand> _commands;
    private readonly ITelegramBotClient _botClient;
    private readonly ICommandHelpService _commandHelpService;

    public HandleTextCommandAsync()
    {
        _commands = Substitute.For<IEnumerable<ICommand>>();
        _botClient = Substitute.For<ITelegramBotClient>();
        _commandHelpService = Substitute.For<ICommandHelpService>();
        _commandService = new CommandService(_commands, _botClient, _commandHelpService);
    }

    [Fact]
    public async Task HandleTextCommandAsync_WhenCommandIsNotFound_Return()
    {
        // Arrange
        const string command = "/some_command";
        await Initialize();
        
        var helpCommand = Substitute.For<ICommand>();
        helpCommand.CommandType.Returns(CommandType.Help);
        
        var commands = new List<ICommand> { helpCommand };
        using var enumerator = commands.GetEnumerator();
        
        _commands.GetEnumerator().Returns(enumerator);

        // Act
        await _commandService.HandleTextCommandAsync(new UserDto(), new ChatDto(), command, default);

        // Assert
        await _commandHelpService.DidNotReceive()
            .DisplayHelpAsync(Arg.Any<long>(), Arg.Any<CommandType>(), Arg.Any<CancellationToken>());
        await helpCommand.DidNotReceive()
            .ExecuteAsync(Arg.Any<UserDto>(), Arg.Any<ChatDto>(), Arg.Any<CancellationToken>());
        await helpCommand.DidNotReceive().ExecuteWithCommandLineArguments(Arg.Any<UserDto>(), Arg.Any<ChatDto>(),
            Arg.Any<Dictionary<string, string>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleTextCommandAsync_WhenWithoutArguments_ExecuteCommandAsync()
    {
        // Arrange
        const string command = "/help";
        await Initialize();
        
        var helpCommand = Substitute.For<ICommand>();
        helpCommand.CommandType.Returns(CommandType.Help);
        
        var commands = new List<ICommand> { helpCommand };
        using var enumerator = commands.GetEnumerator();
        
        _commands.GetEnumerator().Returns(enumerator);

        // Act
        await _commandService.HandleTextCommandAsync(new UserDto(), new ChatDto(), command, default);

        // Assert
        await _commandHelpService.DidNotReceive()
            .DisplayHelpAsync(Arg.Any<long>(), Arg.Any<CommandType>(), Arg.Any<CancellationToken>());
        await helpCommand.Received()
            .ExecuteAsync(Arg.Any<UserDto>(), Arg.Any<ChatDto>(), Arg.Any<CancellationToken>());
        await helpCommand.DidNotReceive().ExecuteWithCommandLineArguments(Arg.Any<UserDto>(), Arg.Any<ChatDto>(),
            Arg.Any<Dictionary<string, string>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleTextCommandAsync_WhenWithArguments_ExecuteCommandWithArguments()
    {
        // Arrange
        const string command = "/get_tasks --argument";
        await Initialize();
        
        var getTasksCommand = Substitute.For<ICommand>();
        getTasksCommand.IsAllowCommandLineArguments.Returns(true);
        getTasksCommand.CommandType.Returns(CommandType.GetTasks);
        
        var commands = new List<ICommand> { getTasksCommand };
        using var enumerator = commands.GetEnumerator();
        
        _commands.GetEnumerator().Returns(enumerator);

        // Act
        await _commandService.HandleTextCommandAsync(new UserDto(), new ChatDto(), command, default);

        // Assert
        await _commandHelpService.DidNotReceive()
            .DisplayHelpAsync(Arg.Any<long>(), Arg.Any<CommandType>(), Arg.Any<CancellationToken>());
        await getTasksCommand.DidNotReceive()
            .ExecuteAsync(Arg.Any<UserDto>(), Arg.Any<ChatDto>(), Arg.Any<CancellationToken>());
        await getTasksCommand.Received().ExecuteWithCommandLineArguments(Arg.Any<UserDto>(), Arg.Any<ChatDto>(),
            Arg.Any<Dictionary<string, string>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleTextCommandAsync_WhenCommandWithHelpArgument_ExecuteDisplayHelpAsync()
    {
        // Arrange
        const string command = "/get_tasks --help";
        await Initialize();
        
        var getTasksCommand = Substitute.For<ICommand>();
        getTasksCommand.IsAllowCommandLineArguments.Returns(true);
        getTasksCommand.CommandType.Returns(CommandType.GetTasks);
        
        var commands = new List<ICommand> { getTasksCommand };
        using var enumerator = commands.GetEnumerator();
        
        _commands.GetEnumerator().Returns(enumerator);

        // Act
        await _commandService.HandleTextCommandAsync(new UserDto(), new ChatDto(), command, default);

        // Assert
        await _commandHelpService.Received()
            .DisplayHelpAsync(Arg.Any<long>(), Arg.Any<CommandType>(), Arg.Any<CancellationToken>());
        await getTasksCommand.DidNotReceive()
            .ExecuteAsync(Arg.Any<UserDto>(), Arg.Any<ChatDto>(), Arg.Any<CancellationToken>());
        await getTasksCommand.DidNotReceive().ExecuteWithCommandLineArguments(Arg.Any<UserDto>(), Arg.Any<ChatDto>(),
            Arg.Any<Dictionary<string, string>>(), Arg.Any<CancellationToken>());
    }
    
    private async Task Initialize()
    {
        _botClient.MakeRequestAsync(Arg.Any<GetMeRequest>()).Returns(new User { Username = "" });
        await _commandService.InitializeAsync();
    }
}