using System.Globalization;
using CJTasksHelperBot.Application.LanguageCode.Queries;
using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using MediatR;

namespace CJTasksHelperBot.Infrastructure.Services;

public class LocalizationService : ILocalizationService
{
    private readonly IMediator _mediator;
    
    public LocalizationService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public void SetLocalization(long? chatId)
    {
        chatId ??= 0;
        
        var result = _mediator.Send(new GetChatLanguageCodeQuery { ChatId = (long)chatId }).GetAwaiter().GetResult();
        
        CultureInfo cultureInfo;

        if (result.IsSuccess)
        {
            var langCode = result.Value!.DisplayName;
            cultureInfo = new CultureInfo(langCode);
        }
        else
        {
            cultureInfo = new CultureInfo(LanguageCodeCustomEnum.Uk.DisplayName);
        }
        
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
    }
}