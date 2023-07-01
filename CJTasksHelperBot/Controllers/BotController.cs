using CJTasksHelperBot.Filters;
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
		public async Task<IActionResult> Post([FromBody] Update update, [FromServices] UpdateHandlers handleUpdateService, 
			CancellationToken cancellationToken)
		{
			await handleUpdateService.HandleUpdateAsync(update, cancellationToken);

			return Ok();
		}
	}
}
