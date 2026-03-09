using Eclipse.Core.Pipelines;

using Telegram.Bot.Types;

namespace Eclipse.Core.Provider;

/// <summary>
/// Retrieves proper <a cref="IPipeline"></a> for current <a cref="Update"></a>
/// </summary>
public interface IPipelineProvider
{
    /// <summary>
    /// Gets the <a cref="IPipeline"></a> for provided <a cref="Update"></a>.
    /// </summary>
    /// <param name="update">The update.</param>
    /// <returns></returns>
    IPipeline Get(Update update);
}
