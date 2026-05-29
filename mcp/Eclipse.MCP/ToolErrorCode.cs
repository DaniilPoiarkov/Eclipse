namespace Eclipse.MCP;

public enum ToolErrorCode
{
    /// <summary>
    /// The transient error (e.g. timeout, network issues) which can be retried.
    /// </summary>
    Transient,

    /// <summary>
    /// The business rule violation error which should not be retried without changing the request parameters (e.g. not eligable, duplicate, insufficient balance).
    /// </summary>
    Business,
    
    /// <summary>
    /// The validation error which occurs when the input data does not meet the required criteria.
    /// </summary>
    Validation,

    /// <summary>
    /// The permission non-retryable error which occurs when the user does not have the necessary permissions to perform the action.
    /// </summary>
    Permission,

    /// <summary>
    /// The uncertain state which should not be automatically retried without user intervention, as the outcome of the request is unknown (e.g. request was processed but response was not received due to network issues).
    /// </summary>
    Uncertain
}
