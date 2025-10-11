using FluentAssertions;

namespace Eclipse.Common.Tests.Pagination;

public class PaginationListTests
{
    [Theory]
    [InlineData(105, 10, 11)]
    [InlineData(10, 5, 2)]
    [InlineData(17, 5, 4)]
    [InlineData(22, 15, 2)]
    public void ToPaginatedList_WhenCreatedWithProperValues_ThenDataPresentInListIsValid(int totalCount, int pageSize, int pagesCount)
    {
        var numbers = Enumerable
            .Range(0, totalCount)
            .ToPaginatedList(totalCount, pageSize);

        numbers.TotalCount.Should().Be(totalCount);
        numbers.Count.Should().Be(totalCount);
        numbers.Items.Count.Should().Be(totalCount);
        numbers.Pages.Should().Be(pagesCount);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(0, 0)]
    [InlineData(-1, 0)]
    [InlineData(-1, -1)]
    public void ToPaginatedList_WhenCalledWithInvalidArgs_ThenArgumentExceptionThrown(int totalCount, int pageSize)
    {
        var action = () => Enumerable
            .Range(0, totalCount)
            .ToPaginatedList(totalCount, pageSize);

        action.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }
}
