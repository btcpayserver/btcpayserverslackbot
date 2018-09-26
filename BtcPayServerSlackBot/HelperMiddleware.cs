using System.Collections.Generic;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Core.Plugins.StandardPlugins;

public class HelperMiddleware : MiddlewareBase
{
    private readonly StatsPlugin _statsPlugin;

    public HelperMiddleware(IMiddleware next, StatsPlugin statsPlugin)
        : base(next)
    {
        this._statsPlugin = statsPlugin;
        this.HandlerMappings = new HandlerMapping[1]
        {
            new HandlerMapping()
            {
                ValidHandles = new IValidHandle[]{ 
                    new ExactMatchHandle("question"), 
                    new ExactMatchHandle("i have a question"),
                    new ExactMatchHandle("I have a question"),
                    new StartsWithHandle("i need help"),
                    new StartsWithHandle("I need help"),
                    new StartsWithHandle("How can I"),
                    new StartsWithHandle("How can i"),
                    new StartsWithHandle("how can I"),
                    new StartsWithHandle("how can i"), 
                    
                },
                Description = "Tries to help blindly",
                EvaluatorFunc = EvaluatorFunc
            }
        };
    }

    private IEnumerable<ResponseMessage> EvaluatorFunc(IncomingMessage incomingMessage, IValidHandle validHandle)
    {
            
        yield return incomingMessage.IndicateTypingOnChannel();
        _statsPlugin.IncrementState("Helper:BlindlyHelped");
        yield return incomingMessage.ReplyToChannel(
            $"Perhaps you should look at https://nbitstack.com/c/btcpayserver and see if there is already an answer for you there, @{incomingMessage.Username}");
           
    }
}