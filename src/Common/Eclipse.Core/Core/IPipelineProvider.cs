using Eclipse.Core.Pipelines;

namespace Eclipse.Core.Core;

/// <summary>
/// Returns <a cref="PipelineBase"></a> which match specified <![CDATA[route]]>
/// </summary>
public interface IPipelineProvider
{
    PipelineBase Get(string route);
}
