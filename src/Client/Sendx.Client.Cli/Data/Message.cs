using System;

namespace Sendx.Client.Cli.Data
{
	public class Message
	{
		// ReSharper disable once MemberCanBePrivate.Global
		// ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
		public Guid Id { get; set; }
		// ReSharper disable once MemberCanBePrivate.Global
		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		public string Content { get; }

		public Message(string content)
		{
			if(string.IsNullOrEmpty(content))
				throw new ArgumentNullException(nameof(content));
			
			Id = Guid.NewGuid();
			Content = content;
		}
	}
}
