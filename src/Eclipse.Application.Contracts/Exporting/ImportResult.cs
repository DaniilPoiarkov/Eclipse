namespace Eclipse.Application.Contracts.Exporting;

public sealed class ImportResult<TRow>
{
    public bool IsSuccess => FailedRows.IsNullOrEmpty();

    public List<TRow> FailedRows { get; set; } = [];
}
