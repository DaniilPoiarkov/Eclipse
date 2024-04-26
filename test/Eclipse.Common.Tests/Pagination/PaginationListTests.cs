using FluentAssertions;

namespace Eclipse.Common.Tests.Pagination;

public class PaginationListTests
{
    [Theory]
    [InlineData(105, 2, 10,  10, 11, 20, 9)]
    [InlineData(10, 1, 5, 5, 2, 5, -1)]
    [InlineData(17, 3, 5, 5, 4, 15, 9)]
    [InlineData(22, 2, 15, 7, 2, 22, 14)]
    public void ToPaginatedList_WhenCreatedWithProperValues_ThenDataPresentInListIsValid(int totalCount, int page, int pageSize, int count, int pagesCount, int lessThan, int geaterThan)
    {
        var numbers = Enumerable
            .Range(0, totalCount)
            .ToPaginatedList(page, pageSize);

        numbers.TotalCount.Should().Be(totalCount);
        numbers.Count.Should().Be(count);
        numbers.Items.Count.Should().Be(count);
        numbers.Pages.Should().Be(pagesCount);
        numbers.Items.All(x => x < lessThan && x > geaterThan).Should().BeTrue();
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    [InlineData(0, 0)]
    [InlineData(-1, 0)]
    [InlineData(-1, -1)]
    public void ToPaginatedList_WhenCalledWithInvalidArgs_ThenArgumentExceptionThrown(int page, int pageSize)
    {
        var totalCount = 15;

        var action = () => Enumerable
            .Range(0, totalCount)
            .ToPaginatedList(page, pageSize);

        action.Should().ThrowExactly<ArgumentException>();
    }
}
