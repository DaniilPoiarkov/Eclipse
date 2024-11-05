using Eclipse.Pipelines.UpdateHandler;

namespace Eclipse.WebAPI.Models;

[Serializable]
public sealed class SwitchHandlerTypeRequest
{
    public HandlerType Type { get; set; }
}
