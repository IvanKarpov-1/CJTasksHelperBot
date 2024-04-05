using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.UserChat.Commands;
using CJTasksHelperBot.Application.UserChat.Queries;
using CJTasksHelperBot.Infrastructure.Services;
using MediatR;

namespace CJTasksHelperBot.Infrastructure.Tests.Unit.Services.UserChatServiceTests;

public class CreateUserChatAsync
{
    private readonly UserChatService _userChatService;
    private readonly IMediator _mediator;

    public CreateUserChatAsync()
    {
        _mediator = Substitute.For<IMediator>();
        _userChatService = new UserChatService(_mediator);
    }

    [Fact]
    public async Task CreateUserChatAsync_WhenCheckExistingAndUserDtoIsExist_Return()
    {
        // Arrange
        var faker = new AutoFaker();
        var resultUserChatDto = faker.Generate<UserChatDto>();
        var result = Result<UserChatDto>.Success(resultUserChatDto);
        _mediator.Send(Arg.Any<GetUserChatQuery>()).Returns(result);
        
        // Act
        await _userChatService.CreateUserChatAsync(default, default, true);

        // Assert
        await _mediator.DidNotReceive().Send(Arg.Any<CreateUserChatCommand>());
    }

    [Fact]
    public async Task CreateUserChatAsync_WhenCheckExistingAndUserDtoIsNotExist_CreateUserChat()
    {
        // Arrange
        var result = new Result<UserChatDto>();
        _mediator.Send(Arg.Any<GetUserChatQuery>()).Returns(result);
        
        // Act
        await _userChatService.CreateUserChatAsync(default, default, true);

        // Assert
        await _mediator.Received().Send(Arg.Any<CreateUserChatCommand>());
    }

    [Fact]
    public async Task CreateUserChatAsync_WhenNotCheckExisting_CreateUserChat()
    {
        // Arrange
        var result = new Result<UserChatDto>();
        _mediator.Send(Arg.Any<GetUserChatQuery>()).Returns(result);
        
        // Act
        await _userChatService.CreateUserChatAsync(default, default, false);

        // Assert
        await _mediator.Received().Send(Arg.Any<CreateUserChatCommand>());
    }
}