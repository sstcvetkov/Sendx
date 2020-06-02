using Microsoft.EntityFrameworkCore;
using Sendx.Data;

namespace Sendx.Api.Messages.Data
{
	public class DataContext : DbContext
	{
		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		public DbSet<Message> Messages { get; set; }
		
		public DataContext(DbContextOptions<DataContext> options)
			: base(options)
		{
		}
	}
}
