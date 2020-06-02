using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Newtonsoft.Json;
using Sendx.Client.Cli.Data;
using Serilog;

namespace Sendx.Client.Cli.Services
{
	public class MessageService : IMessageService
	{
		private readonly string _connectionString;
		private readonly object _connectionLock = new object();
		public MessageService(string connectionString)
		{
			if(string.IsNullOrWhiteSpace(connectionString))
				throw new ArgumentNullException(nameof(connectionString));
			_connectionString = connectionString;
		}
		
		public void AddMessage(Message message)
		{
			if(message == null)
				throw new ArgumentException(nameof(message));

			lock (_connectionLock)
			{
				Log.Debug($"{nameof(AddMessage)}: " +
				          $"{JsonConvert.SerializeObject(message)}");
				using var db = new LiteDatabase(_connectionString);
				db.GetCollection<Message>()
					.Insert(message);
			}
		}

		public ICollection<Message> GetMessages(int page = 0, int pageSize = int.MaxValue)
		{
			lock (_connectionLock)
			{
				Log.Debug($"{nameof(GetMessages)}: " +
				          $"page = {page}, pagesize = {pageSize}");
				using var db = new LiteDatabase(_connectionString);
				return db.GetCollection<Message>()
					.Query().Offset(page * pageSize).Limit(pageSize).ToList();
			}
		}
		
		public void RemoveMessages(ICollection<Message> messages)
		{
			lock (_connectionLock)
			{
				Log.Debug($"{nameof(RemoveMessages)}: " +
				          $"{JsonConvert.SerializeObject(messages)}");
				using var db = new LiteDatabase(_connectionString);
				var collection = db.GetCollection<Message>();
				collection.DeleteMany(x=> messages.Select(m=> m.Id)
					.Any(mid=> mid == x.Id));
			}
		}
	}
}
