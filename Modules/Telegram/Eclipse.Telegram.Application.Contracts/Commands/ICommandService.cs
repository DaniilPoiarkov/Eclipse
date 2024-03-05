using Eclipse.Common.Results;

namespace Eclipse.Telegram.Application.Contracts.Commands;

public interface ICommandService
{
    Task<IReadOnlyList<CommandDto>> GetList(CancellationToken cancellationToken = default);

    Task<Result> Add(AddCommandRequest request, CancellationToken cancellationToken = default);

    Task Remove(string command, CancellationToken cancellationToken = default);
}
