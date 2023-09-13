using Eclipse.Application.Contracts.Base;

namespace Eclipse.Application.Contracts.ChronicleItems;

public class ChronicleItemDto : EntityDto
{
    public long UserId { get; set; }

    public string Text { get; set; } = string.Empty;

    public bool IsFinished { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime? FinishedAt { get; set; }
}
