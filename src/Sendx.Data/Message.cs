using System;

namespace Sendx.Data
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public class Message
	{
		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		public Guid Id { get; set; }
		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		public string Content { get; set; }
		public string ClientIp { get; set; }
		public DateTime Created { get; set; }
	}
}
