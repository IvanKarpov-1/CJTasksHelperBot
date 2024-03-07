using System.Globalization;
using System.Text.Json;
using CJTasksHelperBot.Application.Common.Interfaces;
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
        
        var result = await mediator.Send(new GetUserChatQuery { UserId = userId, ChatId = chatId });
    
        CultureInfo cultureInfo;

        if (result.IsSuccess)
        {
            var langCode = result.Value!.LanguageCode;
            cultureInfo = new CultureInfo(LanguageCodeCustomEnum.FromValue((int)langCode).DisplayName);
        }
        else
        {
            cultureInfo = new CultureInfo(LanguageCodeCustomEnum.EnUs.DisplayName);
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