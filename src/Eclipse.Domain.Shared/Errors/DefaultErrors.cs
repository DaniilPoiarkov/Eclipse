using Eclipse.Common.Results;

namespace Eclipse.Domain.Shared.Errors;

public static class DefaultErrors
{
    public static Error EntityNotFound(Type type) => Error.NotFound("Defaults.EntityNotFound", "Entity:NotFound", type.Name);
}
