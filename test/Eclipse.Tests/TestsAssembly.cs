using Eclipse.WebAPI.Extensions;

using System.Text;

namespace Eclipse.Tests;

public static class TestsAssembly
{
    public static string ToBase64String(string value)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
    }

    public static MemoryStream GetManifestResourceFile(string name)
    {
        using var stream = typeof(TestsAssembly).Assembly
            .GetManifestResourceStream(name)
                ?? throw new InvalidOperationException("File not found.");

        return stream.CreateMemoryStream();
    }
}
