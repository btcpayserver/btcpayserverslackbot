using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Noobot.Core;
using Noobot.Core.Configuration;
using Noobot.Core.DependencyResolution;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace BtcPayServerSlackBot
{
    public class BotService : IHostedService
    {
        private readonly ILogger<BotService> _logger;
        private readonly BotConfiguration _botConfiguration;
        private readonly IConfigReader _configReader;
        private INoobotCore _noobotCore;

        public BotService(
            IConfiguration configuration,
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
}