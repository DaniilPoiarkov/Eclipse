using Eclipse.Common.Excel;

using MiniExcelLibs;

namespace Eclipse.Infrastructure.Excel;

internal sealed class ExcelManager : IExcelManager
{
    public IEnumerable<T> Read<T>(Stream stream)
        where T : class, new()
    {
        return stream.Query<T>();
    }

    public MemoryStream Write<T>(IEnumerable<T> values)
    {
        var stream = new MemoryStream();

        stream.SaveAs(values);

        stream.Position = 0;

        return stream;
    }
}
