using System;
using System.Collections.Generic;
using System.Linq;
using Flurl;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Core.Plugins.StandardPlugins;
using Noobot.Toolbox.Plugins;

public class HelperMiddleware : MiddlewareBase
{
    public class Joke
    {
        public string Content { get; set; }
    }
    
    private readonly StatsPlugin _statsPlugin;
    private readonly JsonStoragePlugin _jsonStoragePlugin;

    public HelperMiddleware(IMiddleware next, StatsPlugin statsPlugin, JsonStoragePlugin jsonStoragePlugin)
        : base(next)
    {
        _r = new Random();
        this._statsPlugin = statsPlugin;
        _jsonStoragePlugin = jsonStoragePlugin;
        this.HandlerMappings = new HandlerMapping[1]
        {
            new HandlerMapping()
            {
                ValidHandles = new[] {new AlwaysMatchHandle()},
                Description = "Gets search result from discourse",
                EvaluatorFunc = DiscourseSearch
            }
        };

        jokes = jsonStoragePlugin.ReadFile<Joke>("jokes").ToList();
        if (!jokes.Any())
        {
            jokes.Add( new Joke()
            {
                Content = "bcash is the superior cryptocurrency because big blocks are cool, I guess"
            });
        }
    }
    
    public static List<Joke> jokes = new List<Joke>()
    {
    };

    private Random _r;


    private IEnumerable<ResponseMessage> DiscourseSearch(IncomingMessage incomingMessage, IValidHandle validHandle)
    {
        var genericAnswerKeywords = new List<string>() {"question", "i have a question", "i need help"};
        if (genericAnswerKeywords.Contains(incomingMessage.RawText.ToLower()))
        {
            yield return incomingMessage.IndicateTypingOnChannel();
            _statsPlugin.IncrementState("Helper:BlindlyHelped");
            yield return incomingMessage.ReplyDirectlyToUser(
                $"Perhaps you should look at https://nbitstack.com/c/btcpayserver and see if there is already an answer for you there, @{incomingMessage.Username}");
        }

        var discourseSearchKeywords = new List<string>() {"how can i", "i need help with"};
        if (discourseSearchKeywords.Any(s => incomingMessage.RawText.ToLower().StartsWith(s.ToLower())))
        {
            var discourseUrl = "https://nbitstack.com";
            var searchText = incomingMessage.RawText;
            discourseSearchKeywords.ForEach(s => { searchText = searchText.Replace(s, ""); });
            yield return incomingMessage.IndicateTypingOnChannel();
            var message = $"Maybe this can help: {discourseUrl}/search?q={Url.Encode(searchText.Trim())}";
            
            _statsPlugin.IncrementState("RedirectedToDiscourseSearch:Count");
            yield return incomingMessage.ReplyDirectlyToUser(message);
        }


        var bcashKeywords = new List<string>() {"bcash", "bitcoin cash", "tell me a joke"};
        if (bcashKeywords.Contains(incomingMessage.RawText.ToLower()))
        {
            
            yield return incomingMessage.IndicateTypingOnChannel();

            _statsPlugin.IncrementState("Jokes:Told");

            yield return incomingMessage.ReplyToChannel(jokes[_r.Next(0, jokes.Count - 1)].Content);
        }
        
        if (incomingMessage.RawText.ToLower().StartsWith("addjoke:"))
        {
            var joke = incomingMessage.RawText.ToLower().Replace("addjoke:", string.Empty);
            jokes.Add(new Joke()
            {
                Content = joke
            });
            _jsonStoragePlugin.SaveFile("jokes", jokes.ToArray());
            yield return incomingMessage.ReplyDirectlyToUser("joke added!");
        }
    }
}