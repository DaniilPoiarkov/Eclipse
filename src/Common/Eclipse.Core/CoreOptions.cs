using Telegram.Bot.Types.Enums;

namespace Eclipse.Core;

public sealed class CoreOptions
{
    /// <summary>
    /// Gets or sets the message persistance in days configuration for <a cref="IMessageStore"></a>.
    /// Default is 3 days.
    /// </summary>
    public int MessagePersistanceInDays { get; set; } = 3;

    /// <summary>
    /// Gets the allowed update types.
    /// </summary>
    public List<UpdateType> AllowedUpdateTypes { get; init; } = [];
}
