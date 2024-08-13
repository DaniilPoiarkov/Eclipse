using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Services;

namespace Eclipse.Application.Exporting;

public interface IImportValidator<TRow, TOptions> : IHasMethodInjection<TOptions>
    where TRow : ImportEntityBase
{
    IEnumerable<TRow> ValidateAndSetErrors(IEnumerable<TRow> rows);
}
