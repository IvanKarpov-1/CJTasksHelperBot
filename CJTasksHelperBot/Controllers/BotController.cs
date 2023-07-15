using CJTasksHelperBot.Filters;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Handlers;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Controllers
{
	[ApiController]
	[Route("bot")]
	public class BotController : ControllerBase
	{
		private readonly IUpdateHandler _updateHandler;

		public BotController(IUpdateHandler updateHandler)
		{
			_updateHandler = updateHandler;
		}

		[HttpPost]
		[ValidateTelegramBot]
		public async Task<IActionResult> Post([FromBody] Update update, CancellationToken cancellationToken)
		{
			await _updateHandler.HandleUpdateAsync(update, cancellationToken);

			return Ok();
		}
	}
}
