using System.Linq.Expressions;
using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.LanguageCode.Commands;

namespace CJTasksHelperBot.Application.Tests.Unit.LanguageCode.Commands;

public class ChangeChatLanguageCodeCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnFailureResult_WhenChatWithIdIsNotFound()
    {
        // Arrange
        var faker = new AutoFaker();
        var chatId = faker.Generate<long>();
        var command = new ChangeChatLanguageCodeCommand { ChatId = chatId };
        var handler = new ChangeChatLanguageCodeCommandHandler(_unitOfWork);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be("Chat with id 0 not found");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnSuccessfulResult_WhenLanguageCodeIsChanged()
    {
        // Arrange
        var faker = new AutoFaker();
        var chat = faker.Generate<Domain.Entities.Chat>();
        chat.LanguageCode = Domain.Enums.LanguageCode.Uk;
        var command = new ChangeChatLanguageCodeCommand { LanguageCode = Domain.Enums.LanguageCode.EnUs };
        _unitOfWork.ChatRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.Chat, bool>>>()).Returns(chat);
        var handler = new ChangeChatLanguageCodeCommandHandler(_unitOfWork);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(MediatR.Unit.Value);
        await _unitOfWork.Received().CommitAsync();
    }
}