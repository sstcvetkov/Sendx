using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace Sendx.Api.Messages.Controllers
{
	[ApiController]
	public class ErrorController : ControllerBase
	{
		private readonly ILogger<MessagesController> _logger;

		public ErrorController(ILogger<MessagesController> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}
		
		[Route("/error")]
		public IActionResult ErrorLocalDevelopment(
			[FromServices] IWebHostEnvironment webHostEnvironment)
		{
			var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();

			_logger.LogError($"Fatal: {feature?.Error?.Message}");
			_logger.LogError(JsonConvert.SerializeObject(feature, 
				new JsonSerializerSettings{ReferenceLoopHandling  = ReferenceLoopHandling.Ignore}));

			return Problem();
		}
	}
}
