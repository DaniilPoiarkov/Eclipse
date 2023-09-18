﻿using Eclipse.Core.Validation;

namespace Eclipse.Core.Core;

/// <summary>
/// Use this interface to override default implementation. You also MUST inherit from <a cref="Pipelines.PipelineBase"></a> class.
/// Use Initialize method to register <a cref="ProccedErrors"></a> method
/// </summary>
public interface IAccessDeniedPipeline
{
    Task<IResult> ProccedErrors(MessageContext context, CancellationToken cancellationToken = default);

    void SetResults(IEnumerable<ValidationResult> results);
}
