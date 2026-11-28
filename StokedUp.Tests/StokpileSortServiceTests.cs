
using System;
using System.Collections.Generic;
using System.Linq;
using StokedUp.Services;
using Xunit;

namespace StokedUp.Tests;

public class StokpileSortServiceTests
{
    // Independent sorting tests.
    private class TestItem
    {
        public string Name { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
    }

    // Create sample grocery items in an unsorted order.
    private static List<TestItem> CreateTestItems()
    {
        return new List<TestItem>
        {
            new TestItem
            {
                Name = "Milk",
                ExpirationDate = new DateTime(2026, 12, 5)
            },
            new TestItem
            {
                Name = "Apples",
                ExpirationDate = new DateTime(2026, 12, 10)
            },
            new TestItem
            {
                Name = "Bread",
                ExpirationDate = new DateTime(2026, 12, 2)
            }
        };
    }

    // STOK-236: Test alphabetical ascending sorting.
    [Fact]
    public void SortByName_Ascending_ReturnsAlphabeticalOrder()
    {
        // Arrange
        var items = CreateTestItems();

        // Act
        var result = StokpileSortService.SortByName(
            items, item => item.Name, true);

        // Assert
        Assert.Equal(
            new[] { "Apples", "Bread", "Milk" },
            result.Select(item => item.Name));
    }

    // STOK-236: Test alphabetical descending sorting.
    [Fact]
    public void SortByName_Descending_ReturnsReverseOrder()
    {
        var items = CreateTestItems();

        var result = StokpileSortService.SortByName(
            items, item => item.Name, false);

        Assert.Equal(
            new[] { "Milk", "Bread", "Apples" },
            result.Select(item => item.Name));
    }

    // STOK-236: Test case-insensitive sorting.
    [Fact]
    public void SortByName_MixedCase_IgnoresCapitalization()
    {
        var items = new List<TestItem>
        {
            new TestItem { Name = "banana" },
            new TestItem { Name = "Apple" },
            new TestItem { Name = "Carrot" }
        };

        var result = StokpileSortService.SortByName(
            items, item => item.Name, true);

        Assert.Equal(
            new[] { "Apple", "banana", "Carrot" },
            result.Select(item => item.Name));
    }

    // STOK-236: Test expiration date ascending sorting.
    [Fact]
    public void SortByExpirationDate_Ascending_ReturnsEarliestFirst()
    {
        var items = CreateTestItems();

        var result = StokpileSortService.SortByExpirationDate(
            items, item => item.ExpirationDate, true);

        Assert.Equal(
            new[] { "Bread", "Milk", "Apples" },
            result.Select(item => item.Name));
    }

    // STOK-236: Test expiration date descending sorting.
    [Fact]
    public void SortByExpirationDate_Descending_ReturnsLatestFirst()
    {
        var items = CreateTestItems();

        var result = StokpileSortService.SortByExpirationDate(
            items, item => item.ExpirationDate, false);

        Assert.Equal(
            new[] { "Apples", "Milk", "Bread" },
            result.Select(item => item.Name));
    }

    // STOK-236: Test items with identical expiration dates.
    [Fact]
    public void SortByExpirationDate_IdenticalDates_RetainsAllItems()
    {
        var items = CreateTestItems();

        foreach (var item in items)
        {
            item.ExpirationDate = new DateTime(2026, 12, 5);
        }

        var result = StokpileSortService.SortByExpirationDate(
            items, item => item.ExpirationDate, true);

        Assert.Equal(items, result);
    }

    // STOK-236: Test empty lists.
    [Fact]
    public void SortByName_EmptyList_ReturnsEmptyList()
    {
        var items = new List<TestItem>();

        var result = StokpileSortService.SortByName(
            items, item => item.Name);

        Assert.Empty(result);
    }

    [Fact]
    public void SortByExpirationDate_EmptyList_ReturnsEmptyList()
    {
        var items = new List<TestItem>();

        var result = StokpileSortService.SortByExpirationDate(
            items, item => item.ExpirationDate);

        Assert.Empty(result);
    }

    // STOK-236: Test sorting a single item.
    [Fact]
    public void Sort_SingleItem_ReturnsSameItem()
    {
        var item = new TestItem { Name = "Milk" };
        var items = new List<TestItem> { item };

        var nameResult = StokpileSortService.SortByName(
            items, grocery => grocery.Name);

        var dateResult = StokpileSortService.SortByExpirationDate(
            items, grocery => grocery.ExpirationDate);

        Assert.Same(item, Assert.Single(nameResult));
        Assert.Same(item, Assert.Single(dateResult));
    }

    // STOK-236: Test default sorting direction.
    [Fact]
    public void SortByName_DefaultDirection_ReturnsAscendingOrder()
    {
        var items = CreateTestItems();

        var result = StokpileSortService.SortByName(
            items, item => item.Name);

        Assert.Equal(
            new[] { "Apples", "Bread", "Milk" },
            result.Select(item => item.Name));
    }

    [Fact]
    public void SortByExpirationDate_DefaultDirection_ReturnsAscendingOrder()
    {
        var items = CreateTestItems();

        var result = StokpileSortService.SortByExpirationDate(
            items, item => item.ExpirationDate);

        Assert.Equal(
            new[] { "Bread", "Milk", "Apples" },
            result.Select(item => item.Name));
    }

    // STOK-236: Verify sorting does not modify the original list.
    [Fact]
    public void Sort_OriginalList_RemainsUnchanged()
    {
        var items = CreateTestItems();
        var originalOrder = items.ToArray();

        StokpileSortService.SortByName(
            items, item => item.Name);

        Assert.Equal(originalOrder, items);

        StokpileSortService.SortByExpirationDate(
            items, item => item.ExpirationDate);

        Assert.Equal(originalOrder, items);
    }
}