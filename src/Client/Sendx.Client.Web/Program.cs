using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Json;
using System;
using System.IO;

namespace Sendx.Client.Web
{
	public static class Program
	{
		private static IConfiguration _configuration;
		
		public static void Main(string[] args)
		{
			_configuration = new ConfigurationBuilder()
				.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile("appsettings.json")
				.AddCommandLine(args)
				.AddEnvironmentVariables()
				.Build();
			
			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(_configuration)
				.WriteTo.File(new JsonFormatter(),
					Path.Combine("logs", "log.json"),
					shared: true,
					rollingInterval: RollingInterval.Day,
					fileSizeLimitBytes: 10_000_000, rollOnFileSizeLimit: true)
				.WriteTo.ColoredConsole()
				.CreateLogger();
			
			try
			{
				Log.Information("Starting...");
				CreateHostBuilder(args).Build().Run();
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Failed start");
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		// ReSharper disable once MemberCanBePrivate.Global
		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseSerilog()
				.ConfigureAppConfiguration(context =>
				{
					context.AddConfiguration(_configuration);
				})
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder
						.UseStartup<Startup>()
						.UseUrls(_configuration.GetValue<string>("ProxyUrl"));
				});
	}
}
