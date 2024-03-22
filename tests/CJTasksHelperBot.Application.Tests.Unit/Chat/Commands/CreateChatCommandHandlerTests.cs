using CJTasksHelperBot.Application.Chat.Commands;
using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;

namespace CJTasksHelperBot.Application.Tests.Unit.Chat.Commands;

public class CreateChatCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly MapperlyMapper _mapper;

    public CreateChatCommandHandlerTests()
    {
        _mapper = new MapperlyMapper();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnFailureResult_WhenChatDtoIsNull()
    {
        // Arrange
        var command = new CreateChatCommand();
        var handler = new CreateChatCommandHandler(_unitOfWork, _mapper);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be("ChatDto is null");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_AddChatToRepository_IfChatDtoIsNotNull()
    {
        // Arrange
        var faker = new AutoFaker();
        var chatDto = faker.Generate<ChatDto>();
        var command = new CreateChatCommand { ChatDto = chatDto };
        _unitOfWork.CommitAsync().Returns(1);
        var handler = new CreateChatCommandHandler(_unitOfWork, _mapper);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWork.ChatRepository.Received().Add(Arg.Any<Domain.Entities.Chat>());
        await _unitOfWork.Received().CommitAsync();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_AddUserChatToRepository_IfUserIdIsNotNull()
    {
        // Arrange
        var faker = new AutoFaker();
        var chatDto = faker.Generate<ChatDto>();
        var userId = faker.Generate<long>();
        var command = new CreateChatCommand { ChatDto = chatDto, UserId = userId };
        _unitOfWork.CommitAsync().Returns(1);
        var handler = new CreateChatCommandHandler(_unitOfWork, _mapper);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWork.UserChatRepository.Received().Add(Arg.Any<Domain.Entities.UserChat>());
        await _unitOfWork.Received().CommitAsync();
    }
}