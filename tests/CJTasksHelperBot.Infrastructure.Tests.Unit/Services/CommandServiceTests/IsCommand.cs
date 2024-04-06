using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Services;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Tests.Unit.Services.CommandServiceTests;

public class IsCommand
{
    private readonly CommandService _commandService;
    private readonly ITelegramBotClient _botClient;

    public IsCommand()
    {
        var commands = Substitute.For<IEnumerable<ICommand>>();
        _botClient = Substitute.For<ITelegramBotClient>();
        var commandHelpService = Substitute.For<ICommandHelpService>();
        _commandService = new CommandService(commands, _botClient, commandHelpService);
    }

    [Theory]
    [InlineData("some text", false, "")]
    [InlineData("some text with /command", false, "")]
    [InlineData("/start", true, "")]
    [InlineData("/start", true, "myBot")]
    [InlineData("/start --argument", true, "myBot")]
    [InlineData("/start@myBot", true, "myBot")]
    [InlineData("/start@myBot", false, "")]
    [InlineData("/start@anotherBot", false, "myBot")]
    [InlineData("/start@myBot --argument", true, "myBot")]
    public async Task IsCommand_WhenPassedText_ReturnExpectedResult(string text, bool expectedResult, string botName)
    {
        // Arrange
        _botClient.MakeRequestAsync(Arg.Any<GetMeRequest>()).Returns(new User { Username = botName });
        await _commandService.InitializeAsync();

        // Act
        var result = _commandService.IsCommand(text);

        // Assert
        result.Should().Be(expectedResult);
    }
}