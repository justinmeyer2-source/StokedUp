

using System;
using System.Collections.Generic;
using System.Linq;

namespace StokedUp.Services;

public static class StokpileFilterService
{
    // Filter Stokpile items by the selected category.
    public static List<T> FilterByCategory<T>(
        IEnumerable<T> items,
        string? selectedCategory,
        Func<T, string> categorySelector)
    {
        // Return all items when no category is selected.
        if (string.IsNullOrWhiteSpace(selectedCategory))
        {
            return items.ToList();
        }

        // Return items matching the selected category.
        return items
            .Where(item => string.Equals(
                categorySelector(item),
                selectedCategory,
                StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
