using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sendx.Client.Web.Services;
using System;
using System.Net.Http;

namespace Sendx.Client.Web
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
			services.AddRazorPages();
			services.AddServerSideBlazor();
			services.AddTransient(x=> new HttpClient { BaseAddress = 
				new Uri(Configuration.GetValue<string>("BackendApi")) });
			services.AddTransient<IMessageService, MessageService>();
		}

		// ReSharper disable once UnusedMember.Global
		// ReSharper disable once UnusedParameter.Global
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseExceptionHandler(new ExceptionHandlerOptions
			{
				ExceptionHandler = ErrorHandler.HandleError
			});
			app.UseStaticFiles();
			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapBlazorHub();
				endpoints.MapFallbackToPage("/_Host");
			});
		}
	}
}
