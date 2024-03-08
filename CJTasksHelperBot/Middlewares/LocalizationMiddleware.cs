using System.Globalization;
using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.LanguageCode.Queries;
using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Extensions;
using MediatR;
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
        
        long chatId = 0;

        if (body != null)
        {
            chatId = (long)(body.SelectToken("message.chat.id") ?? chatId);
        }

        var result = await mediator.Send(new GetChatLanguageCodeQuery { ChatId = chatId });
        
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