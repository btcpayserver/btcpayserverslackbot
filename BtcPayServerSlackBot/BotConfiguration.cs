using System;
using System.Collections;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BtcPayServerSlackBot;
using Flurl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Noobot.Core.Configuration;
using Noobot.Core.Plugins.StandardPlugins;
using Noobot.Toolbox.Middleware;
using Noobot.Toolbox.Plugins;


public class BotConfiguration : ConfigurationBase
{
    public BotConfiguration()
    {

        UseMiddleware<WelcomeMiddleware>();
        UseMiddleware<HelperMiddleware>();

        UsePlugin<JsonStoragePlugin>();
        UsePlugin<StatsPlugin>();
        
    }

    private void UseMiddleware(params Type[] types)
    {
        var useMiddlewareMethod = typeof(CollectionBase).GetMethod(nameof(UseMiddleware));
        foreach (var type in types)
        {
            var genericMethod = useMiddlewareMethod.MakeGenericMethod(type);
            genericMethod.Invoke(null, null); // No target, no arguments
        }
    }

    private void UsePlugin(params Type[] types)
    {
        var usePluginMethod = typeof(CollectionBase).GetMethod(nameof(UsePlugin));
        foreach (var type in types)
        {
            var genericMethod = usePluginMethod.MakeGenericMethod(type);
            genericMethod.Invoke(null, null); // No target, no arguments
        }
    }
    
}