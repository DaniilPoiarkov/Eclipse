namespace Eclipse.Application.Contracts.ChronicleItems;

public interface IChronicleItemService
{
    IReadOnlyList<ChronicleItemDto> GetUserItems(long userId);

    ChronicleItemDto AddItem(long userId, string text);

    void FinishItem(Guid itemId);
}
