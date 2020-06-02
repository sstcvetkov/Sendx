using Sendx.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sendx.Api.Messages.Services
{
	public interface IMessageService
	{
		Task Add(Message message);
		Task<Message> Get(Guid id);
		Task<ICollection<Message>> Get(int page = 0, int size = int.MaxValue);
	}
}
