namespace Eclipse.Core.Builder;

public sealed class CoreOptions
{
    /// <summary>
    /// Gets or sets the message persistance in days configuration for <a cref="IMessageStore"></a>.
    /// Default is 3 days.
    /// </summary>
    public int MessagePersistanceInDays { get; set; } = 3;
}
