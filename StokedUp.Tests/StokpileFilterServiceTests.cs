

using System.Collections.Generic;
using StokedUp.Services;
using Xunit;

namespace StokedUp.Tests;

public class StokpileFilterServiceTests
{
    // Independent testing.
    private record TestItem(string Name, string Category);

    // Reusable sample data.
    private static List<TestItem> CreateTestItems()
    {
        return new List<TestItem>
        {
            new TestItem("Milk", "Dairy"),
            new TestItem("Chicken", "Meat"),
            new TestItem("Cheese", "Dairy"),
            new TestItem("Apple", "Fresh Produce")
        };
    }

    // STOK-229: Test filtering with different categories.
    [Theory]
    [InlineData("Dairy", 2)]
    [InlineData("Meat", 1)]
    [InlineData("Fresh Produce", 1)]
    public void FilterByCategory_ValidCategory_ReturnsMatchingItems(
        string category, int expectedCount)
    {
        // Arrange
        var items = CreateTestItems();

        // Act
        var result = StokpileFilterService.FilterByCategory(
            items,
            category,
            item => item.Category
        );

        // Assert
        Assert.Equal(expectedCount, result.Count);
        Assert.All(result, item => Assert.Equal(category, item.Category));
    }

    // STOK-229: Test a category with no matching items.
    [Fact]
    public void FilterByCategory_NoMatchingItems_ReturnsEmptyList()
    {
        // Arrange
        var items = CreateTestItems();

        // Act
        var result = StokpileFilterService.FilterByCategory(
            items,
            "Frozen",
            item => item.Category
        );

        // Assert
        Assert.Empty(result);
    }


    // STOK-229: Test clearing the category filter.
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void FilterByCategory_ClearFilter_ReturnsAllItems(
        string? category)
    {
        // Arrange
        var items = CreateTestItems();

        // Act
        var result = StokpileFilterService.FilterByCategory(
            items,
            category,
            item => item.Category
        );

        // Assert
        Assert.Equal(items.Count, result.Count);
        Assert.Equal(items, result);
    }

    // STOK-229: Test case-insensitive filtering.
    [Theory]
    [InlineData("dairy")]
    [InlineData("DAIRY")]
    [InlineData("DaIrY")]
    public void FilterByCategory_DifferentCapitalization_ReturnsMatchingItems(
        string category)
    {
        // Arrange
        var items = CreateTestItems();

        // Act
        var result = StokpileFilterService.FilterByCategory(
            items,
            category,
            item => item.Category
        );

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, item => Assert.Equal("Dairy", item.Category));
    }

    // STOK-229: Test an empty Stokpile.
    [Fact]
    public void FilterByCategory_EmptyCollection_ReturnsEmptyList()
    {
        // Arrange
        var items = new List<TestItem>();

        // Act
        var result = StokpileFilterService.FilterByCategory(
            items,
            "Dairy",
            item => item.Category
        );

        // Assert
        Assert.Empty(result);
    }

    // STOK-229: Verify the original collection is preserved.
    [Fact]
    public void FilterByCategory_DoesNotModifyOriginalCollection()
    {
        // Arrange
        var items = CreateTestItems();
        var originalItems = new List<TestItem>(items);

        // Act
        var result = StokpileFilterService.FilterByCategory(
            items,
            "Dairy",
            item => item.Category
        );

        // Assert
        Assert.Equal(originalItems, items);
        Assert.Equal(2, result.Count);
        Assert.NotSame(items, result);
    }


}
