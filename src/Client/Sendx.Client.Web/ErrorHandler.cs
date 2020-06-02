using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System.Threading.Tasks;

namespace Sendx.Client.Web
{
	public static class ErrorHandler
	{
#pragma warning disable 1998
		public static async Task HandleError(HttpContext context)
#pragma warning restore 1998
		{
			var feature = context.Features.Get<IExceptionHandlerFeature>();
			
			Log.Fatal($"Fatal: {feature?.Error?.Message}");
			Log.Fatal(JsonConvert.SerializeObject(feature, 
				new JsonSerializerSettings{ReferenceLoopHandling  = ReferenceLoopHandling.Ignore}));    
		}
	}
}
