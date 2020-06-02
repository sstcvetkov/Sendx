using Sendx.Data;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Sendx.Client.Web.Services
{
	public class MessageService : IMessageService
	{
		private readonly HttpClient _httpClient;
		private const string _endpoint = "messages";

		public MessageService(HttpClient httpClient)
		{
			_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		}

		public async Task<ICollection<Message>> GetMessages(int page = 0, int size = int.MaxValue)
		{
			var data = await _httpClient.GetFromJsonAsync<List<Message>>
			($"{_endpoint}?page={page}&size={size}");

			return data;
		}
	}
}
