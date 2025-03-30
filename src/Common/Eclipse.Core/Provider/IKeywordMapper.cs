namespace Eclipse.Core.Provider;

public interface IKeywordMapper
{
    /// <summary>
    /// Allows you to map value from <a cref="Telegram.Bot.Types.Message.Text"></a> to unified keyword.
    /// </summary>
    /// <param name="keyword">The keyword.</param>
    /// <returns></returns>
    string Map(string keyword);
}
