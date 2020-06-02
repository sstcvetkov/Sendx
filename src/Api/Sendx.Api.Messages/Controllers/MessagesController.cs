using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sendx.Data;
using Sendx.Api.Messages.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sendx.Api.Messages.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class MessagesController : ControllerBase
	{
		private readonly IMessageService _messageService;
		private readonly ILogger<MessagesController> _logger;

		public MessagesController(IMessageService messageService, ILogger<MessagesController> logger)
		{
			_messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		public async Task<IEnumerable<Message>> Get(
			[FromQuery]int page = 0, [FromQuery]int size = int.MaxValue)
		{
			return await _messageService.Get(page, size);
		}
		
		[HttpPost]
		public async Task<IActionResult> Add([FromBody]ICollection<Message> messages)
		{
			_logger.LogDebug(Newtonsoft.Json.JsonConvert.SerializeObject(messages));
			foreach (var message in messages)
			{
				if (await _messageService.Get(message.Id) == null)
				{
					message.ClientIp = Request.HttpContext.Connection
						.RemoteIpAddress.MapToIPv4().ToString();
					message.Created = DateTime.Now;
				
					await _messageService.Add(message);
				}
				else
				{
					_logger.LogWarning($"Try to add existing message: " 
					                   + Newtonsoft.Json.JsonConvert.SerializeObject(message));
				}
			}
	
			return Ok();
		}
	}
}
