using System.Globalization;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.LanguageCode.Queries;
using CJTasksHelperBot.Domain.Entities;
using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Infrastructure.Services;
using MediatR;

namespace CJTasksHelperBot.Infrastructure.Tests.Unit.Services.LocalizationServiceTests;

public class SetLocalization
{
    private readonly LocalizationService _localizationService;
    private readonly IMediator _mediator;

    public SetLocalization()
    {
        _mediator = Substitute.For<IMediator>();
        _localizationService = new LocalizationService(_mediator);
    }

    [Fact]
    public void SetLocalization_WhenChatIdIsNull_SetCultureInfoToUk()
    {
        // Arrange
        var expectedCultureInfo = new CultureInfo(LanguageCodeCustomEnum.Uk.DisplayName);
        _mediator.Send(Arg.Any<GetChatLanguageCodeQuery>())
            .Returns(Result<LanguageCodeCustomEnum>.Failure([]));

        // Act
        _localizationService.SetLocalization(default);

        // Assert
        CultureInfo.CurrentCulture.Should().Be(expectedCultureInfo);
    }

    [Fact]
    public void SetLocalization_WhenChatIdIsNotNullAndChatIsNotFound_SetCultureInfoToUk()
    {
        // Arrange
        const long chatId = 123;
        var expectedCultureInfo = new CultureInfo(LanguageCodeCustomEnum.Uk.DisplayName);
        _mediator.Send(Arg.Any<GetChatLanguageCodeQuery>())
            .Returns(Result<LanguageCodeCustomEnum>.Failure([]));

        // Act
        _localizationService.SetLocalization(chatId);

        // Assert
        CultureInfo.CurrentCulture.Should().Be(expectedCultureInfo);
    }

    [Fact]
    public void SetLocalization_WhenChatIsFound_SetCultureInfoToChatsPreferences()
    {
        // Arrange
        var faker = new AutoFaker();
        var chat = faker.Generate<Chat>();
        var expectedLangCode = LanguageCodeCustomEnum.FromValue((int)chat.LanguageCode);
        var expectedCultureInfo = new CultureInfo(expectedLangCode.DisplayName);
        _mediator.Send(Arg.Any<GetChatLanguageCodeQuery>())
            .Returns(Result<LanguageCodeCustomEnum>.Success(expectedLangCode));

        // Act
        _localizationService.SetLocalization(chat.Id);

        // Assert
        CultureInfo.CurrentCulture.Should().Be(expectedCultureInfo);
    }
}