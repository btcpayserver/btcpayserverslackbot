using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Noobot.Core.Configuration;

namespace BtcPayServerSlackBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<BotConfiguration>();
                    services.AddSingleton<IConfigReader, ConfigReader>();
                    services.AddHostedService<BotService>();
                })
                .ConfigureAppConfiguration(builder =>
                    builder
                        .AddCommandLine(args)
                        .AddEnvironmentVariables()
                )
                .ConfigureLogging(builder => builder
                    .AddDebug()
                    .AddConsole())
                .UseConsoleLifetime()
                .RunConsoleAsync();
        }
    }
}