using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Seekatar.Tools;

namespace WebSettingsApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .InsertSharedDevSettings(reloadOnChange: false, System.Environment.GetEnvironmentVariable("CONFIG_FILE"))
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
