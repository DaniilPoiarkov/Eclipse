using Eclipse.Core.Pipelines;

namespace Eclipse.Core.Provider;

/// <summary>
/// Returns <a cref="PipelineBase"></a> which match specified <![CDATA[route]]>
/// </summary>
public interface IPipelineProvider
{
    PipelineBase Get(string route);
}
