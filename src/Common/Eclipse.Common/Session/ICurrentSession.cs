namespace Eclipse.Common.Session;

public interface ICurrentSession
{
    Guid UserId { get; }

    long ChatId { get; }
}
