using System.Globalization;
using System.Text.Json;
using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.LanguageCode.Queries;
using CJTasksHelperBot.Application.UserChat.Queries;
using CJTasksHelperBot.Domain.Entities;
using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Extensions;
using MediatR;
using Telegram.Bot.Types;
using Task = System.Threading.Tasks.Task;

namespace CJTasksHelperBot.Middlewares;

public class LocalizationMiddleware
{
    private readonly RequestDelegate _next;

    public LocalizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IMediator mediator, IUnitOfWork unitOfWork)
    {
        var body = await context.Request.GetRequestBodyAsync();
        
        long userId = 0;
        long chatId = 0;

        if (body != null)
        {
            userId = (long)(body.SelectToken("message.from.id") ?? userId);
            chatId = (long)(body.SelectToken("message.chat.id") ?? chatId);
        }

        Result<LanguageCodeCustomEnum> result;
        
        if (userId == chatId)
        {
            result = await mediator.Send(new GetUserLanguageCodeQuery { UserId = userId });
        }
        else
        {
            result = await mediator.Send(new GetChatLanguageCodeQuery { ChatId = userId });
        }
    
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

        await _next(context);
    }
}

public static class LocalizationMiddlewareExtensions
{
    public static IApplicationBuilder UseLocalizationMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LocalizationMiddleware>();
    }
}