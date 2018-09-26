using Microsoft.Extensions.Configuration;
using Noobot.Core.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace BtcPayServerSlackBot
{
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
        public bool HelpEnabled => _configuration.GetValue<bool?>(nameof(HelpEnabled)) ?? true;
        public bool StatsEnabled => _configuration.GetValue<bool?>(nameof(StatsEnabled)) ?? true;
        public bool AboutEnabled => _configuration.GetValue<bool?>(nameof(AboutEnabled)) ?? true;
    }
}