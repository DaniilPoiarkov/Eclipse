using Eclipse.Common.Results;

using Microsoft.Extensions.Localization;

namespace Eclipse.Domain.Shared.Errors;

public static class DefaultErrors
{
    public static Error EntityNotFound(Type type, IStringLocalizer localizer) => Error.NotFound("Defaults.EntityNotFound", localizer["Entity:NotFound", type.Name], type.Name);

    public static Error EntityNotFound(Type type) => Error.NotFound("Defaults.EntityNotFound", "Entity:NotFound", type.Name);

    public static Error EntityNotFound<T>() => EntityNotFound(typeof(T));
}
