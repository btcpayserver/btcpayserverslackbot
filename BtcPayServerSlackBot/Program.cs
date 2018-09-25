using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Noobot.Core;
using Noobot.Core.Configuration;
using Noobot.Core.DependencyResolution;
using Noobot.Core.Plugins.StandardPlugins;
using Noobot.Toolbox.Middleware;
using Noobot.Toolbox.Plugins;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

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


    public class BotService : IHostedService
    {
        private readonly ILogger<BotService> _logger;
        private readonly BotConfiguration _botConfiguration;
        private readonly IConfigReader _configReader;
        private INoobotCore _noobotCore;

        public BotService(IConfiguration configuration,
            ILogger<BotService> logger,
            BotConfiguration botConfiguration,
            IConfigReader configReader)
        {
            _logger = logger;
            _botConfiguration = botConfiguration;
            _configReader = configReader;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            IContainerFactory containerFactory = new ContainerFactory(_botConfiguration,
                _configReader, LogManager.GetLogger(GetType()));
            INoobotContainer container = containerFactory.CreateContainer();
            _noobotCore = container.GetNoobotCore();

            _logger.LogInformation("Connecting..");
            await _noobotCore.Connect();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Disconnecting..");
            _noobotCore?.Disconnect();
            return Task.CompletedTask;
        }
    }

    public class ConfigReader : IConfigReader
    {
        private readonly IConfiguration _configuration;

        public ConfigReader(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public T GetConfigEntry<T>(string entryName)
        {
            return _configuration.GetValue<T>(entryName);
        }

        public string SlackApiKey => _configuration.GetValue<string>(nameof(SlackApiKey));
        public bool HelpEnabled => _configuration.GetValue<bool>(nameof(HelpEnabled));
        public bool StatsEnabled => _configuration.GetValue<bool>(nameof(StatsEnabled));
        public bool AboutEnabled => _configuration.GetValue<bool>(nameof(AboutEnabled));
    }
}

public class BotConfiguration : ConfigurationBase
{
    public BotConfiguration()
    {
        UseMiddleware<WelcomeMiddleware>();
        UseMiddleware<JokeMiddleware>();

        UsePlugin<JsonStoragePlugin>();
        UsePlugin<StatsPlugin>();
    }
}

