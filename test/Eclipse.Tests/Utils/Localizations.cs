namespace Eclipse.Tests.Utils;

internal class Localizations
{
    public static readonly Dictionary<string, string> Default = new()
    {
        ["{0}NotFound"] = "{0} not found",
        ["{0}AlreadyExists{1}{2}"] = "{0} with {1} \'{2}\' already exists",
        ["InvalidField{0}{1}"] = "Invalid field {0} \'{1}\'"
    };
}
