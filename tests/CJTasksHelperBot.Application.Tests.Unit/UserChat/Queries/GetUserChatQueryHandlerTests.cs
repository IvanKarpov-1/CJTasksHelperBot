using System.Linq.Expressions;
using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.UserChat.Queries;

namespace CJTasksHelperBot.Application.Tests.Unit.UserChat.Queries;

public class GetUserChatQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly MapperlyMapper _mapper;

    public GetUserChatQueryHandlerTests()
    {
        _mapper = new MapperlyMapper();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenUserChatIsNotFound_ReturnFailureResult()
    {
        // Arrange
        var faker = new AutoFaker();
        var userId = faker.Generate<long>();
        var chatId = faker.Generate<long>();
        var query = new GetUserChatQuery { UserId = userId, ChatId = chatId };
        var handler = new GetUserChatQueryHandler(_unitOfWork, _mapper);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be($"UserChat with UserId {userId} and ChatId {chatId} not found");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenUserChatIsFound_ReturnUserChatDto()
    {
        // Arrange
        var faker = new AutoFaker();
        var userId = faker.Generate<long>();
        var chatId = faker.Generate<long>();
        var userChat = faker.Generate<Domain.Entities.UserChat>();
        var userChatDto = _mapper.Map(userChat);
        var query = new GetUserChatQuery { UserId = userId, ChatId = chatId };
        _unitOfWork.UserChatRepository
            .FindAsync(Arg.Any<Expression<Func<CJTasksHelperBot.Domain.Entities.UserChat, bool>>>(), false)
            .Returns(userChat);
        var handler = new GetUserChatQueryHandler(_unitOfWork, _mapper);
        
        // Act
        var result = await handler.Handle(query, default);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(userChatDto);
    }
}