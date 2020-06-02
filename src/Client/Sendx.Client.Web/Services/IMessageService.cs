using Sendx.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sendx.Client.Web.Services
{
	public interface IMessageService
	{
		// ReSharper disable twice UnusedParameter.Global
		Task<ICollection<Message>> GetMessages(int page = 0, int size = int.MaxValue);
	}
}
