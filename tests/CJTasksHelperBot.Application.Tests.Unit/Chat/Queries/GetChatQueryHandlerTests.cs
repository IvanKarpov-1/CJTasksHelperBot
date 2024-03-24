using System.Linq.Expressions;
using CJTasksHelperBot.Application.Chat.Queries;
using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;

namespace CJTasksHelperBot.Application.Tests.Unit.Chat.Queries;

public class GetChatQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly MapperlyMapper _mapper;

    public GetChatQueryHandlerTests()
    {
        _mapper = new MapperlyMapper();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnFailureResult_WhenChatIdIsNotProvided()
    {
        // Arrange
        var faker = new AutoFaker();
        var chatId = faker.Generate<long>();
        var query = new GetChatQuery { ChatId = chatId };
        var handler = new GetChatQueryHandler(_unitOfWork, _mapper);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be($"Chat with id {chatId} not found");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnFailureResult_WhenChatWithIdIsNotFound()
    {
        // Arrange
        var faker = new AutoFaker();
        var chatId = faker.Generate<long>();
        var query = new GetChatQuery { ChatId = chatId };
        var handler = new GetChatQueryHandler(_unitOfWork, _mapper);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be($"Chat with id {chatId} not found");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnChatDto_WhenChatWithIdIsFound()
    {
        // Arrange
        var faker = new AutoFaker();
        var chatId = faker.Generate<long>();
        var chat = faker.Generate<Domain.Entities.Chat>();
        var chatDto = _mapper.Map(chat);
        var query = new GetChatQuery { ChatId = chatId };
        _unitOfWork.ChatRepository
            .FindAsync(Arg.Any<Expression<Func<CJTasksHelperBot.Domain.Entities.Chat, bool>>>(), false)
            .Returns(chat);
        var handler = new GetChatQueryHandler(_unitOfWork, _mapper);
        
        // Act
        var result = await handler.Handle(query, default);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(chatDto);
    }
}