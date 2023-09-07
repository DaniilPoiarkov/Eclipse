namespace Eclipse.Application.Contracts.Telegram.BotManagement;

public interface ICommandService
{
    Task<IReadOnlyList<CommandDto>> GetList(CancellationToken cancellationToken = default);

    Task Add(CommandDto command, CancellationToken cancellationToken = default);

    Task Remove(string command, CancellationToken cancellationToken = default);
}
