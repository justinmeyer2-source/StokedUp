
using System;
using System.Collections.Generic;
using System.Linq;

namespace StokedUp.Services;

public static class StokpileSortService
{
    // STOK-232: Sort Stokpile items by name.
    public static List<T> SortByName<T>(
        IEnumerable<T> items,
        Func<T, string> nameSelector,
        bool ascending = true)
    {
        // STOK-234: Ascending and descending sorting options.
        return ascending
            ? items.OrderBy(
                nameSelector,
                StringComparer.OrdinalIgnoreCase).ToList()
            : items.OrderByDescending(
                nameSelector,
                StringComparer.OrdinalIgnoreCase).ToList();
    }

    // STOK-233: Sort Stokpile items by expiration date.
    public static List<T> SortByExpirationDate<T>(
        IEnumerable<T> items,
        Func<T, DateTime> expirationDateSelector,
        bool ascending = true)
    {
        // STOK-234: Ascending and descending sorting options.
        return ascending
            ? items.OrderBy(expirationDateSelector).ToList()
            : items.OrderByDescending(expirationDateSelector).ToList();
    }
}
