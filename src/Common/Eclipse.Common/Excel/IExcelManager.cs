namespace Eclipse.Common.Excel;

public interface IExcelManager
{
    IEnumerable<T> Read<T>(Stream stream)
        where T : class, new();

    MemoryStream Write(IEnumerable<Dictionary<string, object>> values);

    MemoryStream Write<T>(IEnumerable<T> values);
}
