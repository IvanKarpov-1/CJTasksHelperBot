using CJTasksHelperBot.Infrastructure.Common.Interfaces.Handlers;
using CJTasksHelperBot.Infrastructure.Handlers;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Tests.Unit.Handlers.UpdateHandlerTests;

public class HandleUpdateAsync
{
    private readonly UpdateHandler _updateHandler;
    private readonly MockLogger<UpdateHandler> _logger;
    private readonly IMessageHandler _messageHandler;
    private readonly ICallbackQueryHandler _callbackQueryHandler;
    
    public HandleUpdateAsync()
    {
        _logger = Substitute.For<MockLogger<UpdateHandler>>();
        _messageHandler = Substitute.For<IMessageHandler>();
        _callbackQueryHandler = Substitute.For<ICallbackQueryHandler>();
        _updateHandler = Substitute.For<UpdateHandler>(_logger, _messageHandler, _callbackQueryHandler);
    }
    
    [Fact]
    public async Task HandleUpdateAsync_WhenUpdateIsNull_LogWarning()
    {
        // Act
        await _updateHandler.HandleUpdateAsync(null, default);

        // Assert
        _logger.Received().Log(LogLevel.Warning, "Update is Null");
        await _messageHandler.DidNotReceive().HandleMessageAsync(Arg.Any<Message>(), default);
        await _callbackQueryHandler.DidNotReceive().HandleCallbackQueryAsync(Arg.Any<CallbackQuery>(), default);
    }

    [Fact]
    public async Task HandleUpdateAsync_WhenUnknownUpdateType_ThrowException()
    {
        // Arrange
        var update = new Update();

        // Act
        await _updateHandler.HandleUpdateAsync(update, default);

        // Assert
        _logger.Received().Log(LogLevel.Error, Arg.Any<string>());
        await _messageHandler.DidNotReceive().HandleMessageAsync(Arg.Any<Message>(), default);
        await _callbackQueryHandler.DidNotReceive().HandleCallbackQueryAsync(Arg.Any<CallbackQuery>(), default);
    }

    [Fact]
    public async Task HandleUpdateAsync_WhenMessageUpdateType_ExecuteHandleMessageAsync()
    {
        // Arrange
        var update = new Update
        {
            Message = new Message()
        };

        // Act
        await _updateHandler.HandleUpdateAsync(update, default);

        // Assert
        await _messageHandler.Received().HandleMessageAsync(Arg.Is(update.Message), default);
        await _callbackQueryHandler.DidNotReceive().HandleCallbackQueryAsync(Arg.Any<CallbackQuery>(), default);
    }

    [Fact]
    public async Task HandleUpdateAsync_WhenCallbackQueryUpdateType_ExecuteHandleCallbackQueryAsync()
    {
        // Arrange
        var update = new Update
        {
            CallbackQuery = new CallbackQuery()
        };

        // Act
        await _updateHandler.HandleUpdateAsync(update, default);

        // Assert
        await _callbackQueryHandler.Received().HandleCallbackQueryAsync(Arg.Is(update.CallbackQuery), default);
        await _messageHandler.DidNotReceive().HandleMessageAsync(Arg.Any<Message>(), default);
    }
}