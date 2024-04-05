using CJTasksHelperBot.Application.Chat.Queries;
using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;

namespace CJTasksHelperBot.Application.Tests.Unit.Chat.Queries;

public class GetChatsWithTasksQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly MapperlyMapper _mapper;

    public GetChatsWithTasksQueryHandlerTests()
    {
        _mapper = new MapperlyMapper();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenChatsWithTasksForUserAreNotFound_ReturnFailureResult()
    {
        // Arrange
        var faker = new AutoFaker();
        var userId = faker.Generate<long>();
        var query = new GetChatsWithTasksQuery{ UserId = userId };
        var handler = new GetChatsWithTasksQueryHandler(_unitOfWork, _mapper);
        _unitOfWork.ChatRepository.GetChatsWithTasksAsync(Arg.Any<long>(), default).Returns([]);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be($"Chats with tasks for User Id {userId} not found");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenChatsWithTasksForUserAreFoundReturnChatDtos()
    {
        // Arrange
        var faker = new AutoFaker();
        var chats = faker.Generate<Domain.Entities.Chat>(5);
        var chatDtos = chats.Select(_mapper.Map);
        var query = new GetChatsWithTasksQuery();var handler = new GetChatsWithTasksQueryHandler(_unitOfWork, _mapper);
        _unitOfWork.ChatRepository.GetChatsWithTasksAsync(Arg.Any<long>(), default).Returns(chats);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(chatDtos);
    }
}