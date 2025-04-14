namespace Eclipse.Common.Excel;

public interface IExcelManager
{
    IEnumerable<T> Read<T>(Stream stream)
        where T : class, new();

    MemoryStream Write<T>(IEnumerable<T> values);
}
