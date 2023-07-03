using CJTasksHelperBot.Filters;
using CJTasksHelperBot.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Controllers
{
	[ApiController]
	[Route("bot")]
	public class BotController : ControllerBase
	{
		[HttpPost]
		[ValidateTelegramBot]
		public async Task<IActionResult> Post([FromBody] Update update, [FromServices] UpdateHandler updateHandler, 
			CancellationToken cancellationToken)
		{
			await updateHandler.HandleUpdateAsync(update, cancellationToken);

			return Ok();
		}
	}
}
