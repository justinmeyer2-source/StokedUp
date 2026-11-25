using Android.Net;
using Android.Webkit;
using SQLite;
using StokedUp.Models;

namespace StokedUp.Data
{
    internal static class LocalDatabase
    {
        private static readonly SQLiteAsyncConnection Database =
            new SQLiteAsyncConnection(Path.Combine
                (FileSystem.AppDataDirectory, "stokedup.db3"));

        private static readonly SemaphoreSlim InitializationLock = new(1, 1);

        private static bool initialized;

        private static readonly string[] AvailableCategories =
        {
            "Fresh Produce",
            "Dairy",
            "Meat",
            "Canned",
            "Frozen",
            "Drinks",
            "Grains",
            "Sweets",
            "Oils",
            "Nuts",
            "Condiments",
            "Miscellaneous"
        };

        //initialize database and create tables
        internal static async Task InitializeAsync()
        {
            if (initialized)
                return;

            await InitializationLock.WaitAsync();

            try
            {
                if (initialized)
                    return;

                await Database.CreateTableAsync<StokpileItem>();
                await Database.CreateTableAsync<Category>();
                await AddAvailableCategoriesAsync();

                initialized = true;
            }
            finally
            {  
                InitializationLock.Release(); 
            }
        }

        //add available categories if they do not already exist
        private static async Task AddAvailableCategoriesAsync()
        {
            var existingCategories =
                await Database.Table<Category>().ToListAsync();

            foreach (string categoryType in AvailableCategories)
            {
                bool categoryExists = existingCategories.Any(
                    category => category.CategoryType == categoryType);

                if (!categoryExists)
                {
                    await Database.InsertAsync(
                        new Category {CategoryType = categoryType});
                }
            }
        }

        //retrieve available categories
        internal static async Task<List<Category>>GetCategoriesAsync()
        {
            await InitializeAsync();

            return await Database.Table<Category>()
                .ToListAsync();
        }

        //US-01: Add Grocery Items to Stokpile
        internal static async Task AddStokpileItemAsync(StokpileItem item)
        {
            await InitializeAsync();
            await Database.InsertAsync(item);
        }

        //Retrieve all grocery items from Stokpile
        internal static async Task<List<StokpileItem>>GetStokpileItemsAsync()
        {
            await InitializeAsync();
            return await Database.Table<StokpileItem>()
                .ToListAsync();
        }

        //US-03: Delete Grocery Items from Stokpile
        internal static async Task DeleteStokpileItemAsync(int id)
        {
            await InitializeAsync();
            await Database.DeleteAsync<StokpileItem> (id);
        }
    }
}
