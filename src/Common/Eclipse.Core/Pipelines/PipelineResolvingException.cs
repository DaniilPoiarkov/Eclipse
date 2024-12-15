namespace Eclipse.Core.Pipelines;

public sealed class PipelineResolvingException : Exception
{
    public PipelineResolvingException(string message)
        : base(message) { }
}
