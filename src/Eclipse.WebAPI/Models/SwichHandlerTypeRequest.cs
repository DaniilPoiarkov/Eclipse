using Eclipse.Pipelines.UpdateHandler;

namespace Eclipse.WebAPI.Models;

[Serializable]
public sealed class SwichHandlerTypeRequest
{
    public HandlerType Type { get; set; }
}
