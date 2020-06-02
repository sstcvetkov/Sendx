using System;
using System.Reflection;
using DbUp;
using Microsoft.Extensions.Configuration;

namespace Migrator
{
	internal static class Program
    {
	    public static void Main(string[] args)
	    {
		    var configuration = new ConfigurationBuilder()
			    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
			    .AddCommandLine(args)
			    .AddEnvironmentVariables()
			    .AddUserSecrets(Assembly.GetExecutingAssembly())
			    .Build();
		    
		    var connectionString = configuration.GetConnectionString("DefaultConnection");

		    var migrator =
			    DeployChanges.To
				    .PostgresqlDatabase(connectionString)
				    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
				    .LogToConsole()
				    .Build();

		    var result = migrator.PerformUpgrade();
		    if (!result.Successful)
		    {
			    Console.ForegroundColor = ConsoleColor.Red;
			    Console.WriteLine(result.Error);
			    Console.ResetColor();

			    Environment.Exit(-1);
			}

		    Console.ForegroundColor = ConsoleColor.Green;
		    Console.WriteLine("Success!");
		    Console.ResetColor();
	    }
    }
}