using CJTasksHelperBot.Application.Chat.Commands;
using CJTasksHelperBot.Application.Chat.Queries;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Mapping;
using CJTasksHelperBot.Infrastructure.Services;
using MediatR;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Tests.Unit.Services.ChatServiceTests;

public class GetChatFromTelegramModelAsync
{
    private readonly ChatService _chatService;
    private readonly IMediator _mediator;
    private readonly MapperlyMapper _mapper;

    public GetChatFromTelegramModelAsync()
    {
        _mediator = Substitute.For<IMediator>();
        _mapper = new MapperlyMapper();
        _chatService = new ChatService(_mediator, _mapper);
    }

    [Fact]
    public async Task GetChatFromTelegramModelAsync_WhenChatByIdIsFound_ReturnChatDto()
    {
        // Arrange
        var faker = new AutoFaker();
        var tgChat = faker.Generate<Chat>();
        var resultChatDto = _mapper.Map(tgChat);
        var result = Result<ChatDto>.Success(resultChatDto);
        _mediator.Send(Arg.Any<GetChatQuery>()).Returns(result);
        
        // Act
        var chatDto = await _chatService.GetChatFromTelegramModelAsync(tgChat);

        // Assert
        chatDto.Should().NotBeNull();
        chatDto.Should().BeEquivalentTo(resultChatDto);
        await _mediator.DidNotReceive().Send(Arg.Any<CreateChatCommand>());
    }

    [Fact]
    public async Task GetChatFromTelegramModelAsync_WhenChatByIdIsNotFound_CreateChatAndReturnChatDto()
    {
        // Arrange
        var faker = new AutoFaker();
        var tgChat = faker.Generate<Chat>();
        var resultChatDto = _mapper.Map(tgChat);
        var result = new Result<ChatDto>();
        _mediator.Send(Arg.Any<GetChatQuery>()).Returns(result);
        
        // Act
        var chatDto = await _chatService.GetChatFromTelegramModelAsync(tgChat);
        
        // Assert
        chatDto.Should().NotBeNull();
        chatDto.Should().BeEquivalentTo(resultChatDto);
        await _mediator.Received().Send(Arg.Any<CreateChatCommand>());
    }
}