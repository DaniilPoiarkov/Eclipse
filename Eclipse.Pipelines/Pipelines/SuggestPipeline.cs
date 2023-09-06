using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Infrastructure.Builder;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Pipelines;

[Route("Suggest", "/suggest")]
public class SuggestPipeline : EclipsePipelineBase
{
    private readonly ITelegramBotClient _botClient;

    private readonly InfrastructureOptions _options;

    public SuggestPipeline(ITelegramBotClient botClient, InfrastructureOptions options)
    {
        _botClient = botClient;
        _options = options;
    }

    private static readonly string[] _greetings = new[]
    {
        "Hey!", "Glad to head that!", "Wow!", "Awesome!"
    };

    private static readonly string _info = "{greeting} Feel free to describe your idea or write /cancel to return";

    protected override void Initialize()
    {
        RegisterStage(SendInfo);
        RegisterStage(RecieveIdea);
    }

    protected static IResult SendInfo(MessageContext context)
    {
        var greeting = _greetings[Random.Shared.Next(0, _greetings.Length)];
        return Text(_info.Replace("{greeting}", greeting));
    }

    protected async Task<IResult> RecieveIdea(MessageContext context, CancellationToken cancellationToken = default)
    {
        if (context.Value.Equals("/cancel", StringComparison.CurrentCultureIgnoreCase))
        {
            return Menu(MainMenuButton, "As you wish");
        }
        
        if (string.IsNullOrEmpty(context.Value))
        {
            return Menu(MainMenuButton, "I'm sure that you have excellent thoughts, unfortunately I can understand only text, so I'll appreciate well-known letters😊");
        }

        var options = _options.TelegramOptions;
        var message = $"Suggestion from {context.User.Name}, @{context.User.Username}:" +
            $"\n{context.Value}";

        await _botClient.SendTextMessageAsync(options.Chat, message, cancellationToken: cancellationToken);

        return Menu(MainMenuButton, "Thank you for suggestion! I'll think about it");
    }
}
