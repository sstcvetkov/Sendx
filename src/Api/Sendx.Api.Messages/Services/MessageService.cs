using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sendx.Api.Messages.Data;
using Sendx.Data;

namespace Sendx.Api.Messages.Services
{
	public class MessageService : IMessageService
	{
		private readonly DataContext _dataContext;
		private readonly ILogger<MessageService> _logger;

		public MessageService(DataContext dataContext, ILogger<MessageService> logger)
		{
			_dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task Add(Message message)
		{
			_logger.LogDebug($"{nameof(Add)}: "
			                 + JsonConvert.SerializeObject(message));

			if (message == null)
			{
				throw new ArgumentException(nameof(message));
			}

			await _dataContext.Messages.AddAsync(message);
			await _dataContext.SaveChangesAsync();
		}

		public async Task<ICollection<Message>> Get(int page = 0, int size = int.MaxValue)
		{
			_logger.LogDebug($"{nameof(Get)}");

			var data = await _dataContext.Messages.AsQueryable()
				.Skip(page * size)
				.Take(size).ToListAsync();

			return data;
		}

		public async Task<Message> Get(Guid id)
		{
			_logger.LogDebug($"{nameof(Get)}[{id}]");

			var found = await _dataContext.Messages.FindAsync(id);

			return found;
		}
	}
}
