using System.Collections.Generic;
using Sendx.Client.Cli.Data;

namespace Sendx.Client.Cli.Services
{
	public interface IMessageService
	{
		void AddMessage(Message message);
		ICollection<Message> GetMessages(int page = 0, int pageSize = int.MaxValue);
		void RemoveMessages(ICollection<Message> ids);
	}
}
