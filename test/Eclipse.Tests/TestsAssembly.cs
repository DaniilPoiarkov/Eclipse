using Eclipse.WebAPI.Extensions;

using System.Text;

namespace Eclipse.Tests;

public static class TestsAssembly
{
    private static readonly string _basePath = "Eclipse.Tests.Files";

    public static string ToBase64String(string value)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
    }

    public static MemoryStream GetValidUsersExcelFile()
    {
        return GetManifestResourceFile($"{_basePath}.valid-users.xlsx");
    }

    public static MemoryStream GetInvalidUsersExcelFile()
    {
        return GetManifestResourceFile($"{_basePath}.invalid-users.xlsx");
    }

    //public static MemoryStream GetValidTodoItemsExcelFile()
    //{
    //    return GetManifestResourceFile($"{_basePath}.valid-todo-items.xlsx");
    //}

    //public static MemoryStream GetValidRemindersExcelFile()
    //{
    //    return GetManifestResourceFile($"{_basePath}.valid-reminders.xlsx");
    //}

    public static MemoryStream GetManifestResourceFile(string name)
    {
        using var stream = typeof(TestsAssembly).Assembly
            .GetManifestResourceStream(name)
                ?? throw new InvalidOperationException("File not found.");

        return stream.CreateMemoryStream();
    }
}
