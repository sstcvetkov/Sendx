using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sendx.Api.Messages.Data;
using Sendx.Api.Messages.Services;

namespace Sendx.Api.Messages
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		// ReSharper disable once MemberCanBePrivate.Global
		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddSingleton<IMessageService, MessageService>();
			services.AddDbContext<DataContext>(options =>
				options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
		}

		// ReSharper disable once UnusedMember.Global
		// ReSharper disable once UnusedParameter.Global
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseExceptionHandler("/error");
			app.UseRouting();
			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}
	}
}
