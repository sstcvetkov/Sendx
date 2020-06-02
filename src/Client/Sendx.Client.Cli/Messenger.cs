using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sendx.Client.Cli.Services;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sendx.Client.Cli
{
	public class Messenger : BackgroundService
	{
		private readonly IMessageService _messageService;
		private readonly ILogger<Messenger> _logger;
		private readonly int _batchSize;
		private readonly HttpClient _httpClient;
		private readonly int _delaySeconds;

		public Messenger(IMessageService messageService, HttpClient httpClient, 
			IConfiguration configuration, ILogger<Messenger> logger)
		{
			_messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
			_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_batchSize = (configuration ?? throw new ArgumentNullException(nameof(configuration)))
				.GetValue<int>("MessageBatchSize");
			_delaySeconds = configuration.GetValue<int>("MessengerDelaySeconds");
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(_delaySeconds * 1000, stoppingToken);
				_logger.LogDebug($"{nameof(ExecuteAsync)}: batch started...");
				try
				{
					var messagesToSend = _messageService
						.GetMessages(0, _batchSize);
					if (messagesToSend.Any())
					{
						var request =  new HttpRequestMessage(
							HttpMethod.Post, "messages");
						request.Content = new StringContent(
							JsonConvert.SerializeObject(messagesToSend),
							Encoding.Default, 
							"application/json");
						var response = await _httpClient.SendAsync(
								request, HttpCompletionOption.ResponseContentRead)
							.ConfigureAwait(false);
						if (response.StatusCode == HttpStatusCode.OK)
						{
							_logger.LogDebug($"{nameof(ExecuteAsync)}: batch sent");
							_messageService.RemoveMessages(messagesToSend);
						}
						else
						{
							_logger.LogWarning($"{nameof(ExecuteAsync)}: failed send batch");
							_logger.LogWarning(JsonConvert.SerializeObject(response));
						}

						_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
					}
					else
					{
						_logger.LogDebug($"{nameof(ExecuteAsync)}: batch is empty");
					}
				}
				catch (Exception e)
				{
					_logger.LogWarning(e, "Error request server");
				}
				
				_logger.LogDebug($"{nameof(ExecuteAsync)}: batch finished.");
			}
		}
	}
}
