using System.Linq.Expressions;
using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.LanguageCode.Queries;
using CJTasksHelperBot.Domain.Enums;

namespace CJTasksHelperBot.Application.Tests.Unit.LanguageCode.Queries;

public class GetChatLanguageCodeQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenChatWithIdIsNotFound_ReturnFailureResult()
    {
        // Arrange
        var query = new GetChatLanguageCodeQuery();
        var handler = new GetChatLanguageCodeQueryHandler(_unitOfWork);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be("Chat with id 0 not found");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenChatWithIdIsFound_ReturnLangCode()
    {
        // Arrange
        var faker = new AutoFaker();
        var chat = faker.Generate<Domain.Entities.Chat>();
        var query = new GetChatLanguageCodeQuery();
        var handler = new GetChatLanguageCodeQueryHandler(_unitOfWork);
        _unitOfWork.ChatRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.Chat, bool>>>(), false).Returns(chat);
        var langCode = LanguageCodeCustomEnum.FromValue((int)chat.LanguageCode);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(langCode);
    }
}