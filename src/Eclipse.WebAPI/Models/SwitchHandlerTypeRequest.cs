using Eclipse.Core.Handlers;

namespace Eclipse.WebAPI.Models;

[Serializable]
public sealed class SwitchHandlerTypeRequest
{
    public HandlerType Type { get; set; }
}
