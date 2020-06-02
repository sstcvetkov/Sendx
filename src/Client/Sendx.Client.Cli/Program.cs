using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sendx.Client.Cli.Data;
using Sendx.Client.Cli.Services;
using Serilog;
using Serilog.Formatting.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;

namespace Sendx.Client.Cli
{
	public static class Program
	{
		private static IConfigurationRoot _configuration;
		private static readonly CancellationToken _messengerCancellationToken
			= new CancellationToken();
		private static User _user;
		private static IMessageService _messageService;
		private static HttpClient _httpClient;
		
		// ReSharper disable once UnusedParameter.Global
		public static void Main(string[] args)
		{
			Greeting();
			try
			{
				InitialiseConfiguration();
				InitialiseLogger();
				InitialiseUser();
				InitialiseMessenger();

				Console.WriteLine("Please enter your messages:");
				while (true)
				{
					var input = Console.ReadLine();
					if (string.IsNullOrEmpty(input))
						continue;
					_messageService.AddMessage(new Message(input));
				}
			}
			catch (Exception e)
			{
				Log.Fatal(e, "Fatal error");
				Console.WriteLine($"Something went wrong: {e.Message}");
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		private static void Greeting()
		{
			var assembly = Assembly.GetEntryAssembly();
			var version = assembly
				?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
				?.InformationalVersion;
			var name = assembly?.GetName().Name;
			Console.Title = $"{name} {version}";
			Console.WriteLine($"Started at {DateTime.Now}.");
		}

		private static bool IsDataBaseNeedCredentials()
		{
			try
			{
				using var db = new LiteDatabase(GetConnectionString());
				return false;
			}
			catch(LiteException e)
			{
				if (e.Message == "This data file is encrypted and needs a password to open")
					return true;
				throw;
			}
		}

		private static void InitialiseConfiguration()
		{
			try
			{
				_configuration = new ConfigurationBuilder()
					.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
					.AddJsonFile("appsettings.json")
					.Build();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Failed loading configuration: {e.Message}");
				Environment.Exit(0);
			}
		}
		
		private static string GetConnectionString()
		{
			var @base = _configuration.GetValue<string>("ConnectionString");
			return _user?.Password == null ? @base : @base + $"Password={_user.Password}";
		}
		
		private static void InitialiseLogger()
		{
			try
			{
				Log.Logger = new LoggerConfiguration()
					.ReadFrom.Configuration(_configuration)
					.WriteTo.File(new JsonFormatter(),
						Path.Combine("logs", "log.json"),
						shared: true,
						rollingInterval: RollingInterval.Day,
						fileSizeLimitBytes: 10_000_000, rollOnFileSizeLimit: true)
					.CreateLogger();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Warning: failed creating logger: {e.Message}");
			}
		}

		private static void InitialiseUser()
		{
			Log.Debug("User initialization...");
			if (!IsDataBaseNeedCredentials()) 
				return;
			
			Log.Debug("Credentials are needed");
			Console.WriteLine("Please enter password for local data encryption(default empty):");
			var user = new User {Password = Console.ReadLine()};
			while (true)
			{
				var context = new ValidationContext(user, null, null);
				var results = new List<ValidationResult>();
				var isValid = Validator.TryValidateObject(user, context, results, true);
				if (!isValid)
				{
					var color = Console.ForegroundColor;
					Console.ForegroundColor = ConsoleColor.DarkRed;
					foreach (var message in results)
						Console.WriteLine("\t" + message.ErrorMessage);
					Console.ForegroundColor = color;
				}
				else
					break;

				Console.WriteLine("You password is not allowed, please try again:");
				user = new User {Password = Console.ReadLine()};
			}
			Log.Information("User has entered credentials");
			_user = user;
		}

		private static void InitialiseMessenger()
		{
			_messageService = new MessageService(GetConnectionString());
			_httpClient = new HttpClient {BaseAddress = new Uri(
				_configuration.GetValue<string>("BackendApi"))};
			Log.Information("Starting the messenger");
			var webHostBuilder =  Host.CreateDefaultBuilder()
				.UseSerilog()
				.ConfigureServices((hostContext, services) =>
				{
					services.AddHostedService<Messenger>();
					services.AddSingleton(_httpClient);
					services.AddSingleton(_messageService);
				});
			
			webHostBuilder.Build().RunAsync(_messengerCancellationToken);
		}
	}
}
