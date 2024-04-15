using FluentAssertions;

namespace Eclipse.Common.Tests.Pagination;

public class PaginationListTests
{
    [Fact]
    public void ToPaginatedList_WhenCreatedWithProperValues_ThenDataPresentInListIsValid()
    {
        var totalCount = 105;
        var page = 2;
        var pageSize = 10;
        var pagesCount = 11;
        var lessThan = 20;
        var geaterThan = 9;

        var numbers = Enumerable
            .Range(0, totalCount)
            .ToPaginatedList(page, pageSize);

        numbers.TotalCount.Should().Be(totalCount);
        numbers.Count.Should().Be(pageSize);
        numbers.Items.Count.Should().Be(pageSize);
        numbers.Pages.Should().Be(pagesCount);
        numbers.Items.All(x => x < lessThan && x > geaterThan).Should().BeTrue();
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    public void ToPaginatedList_WhenCalledWithInvalidArgs_ThenArgumentExceptionThrown(int page, int pageSize)
    {
        var totalCount = 15;

        var action = () => Enumerable
            .Range(0, totalCount)
            .ToPaginatedList(page, pageSize);

        action.Should().ThrowExactly<ArgumentException>();
    }
}
