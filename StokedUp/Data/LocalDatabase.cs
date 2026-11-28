using Android.Net;
using Android.Webkit;
using SQLite;
using AppConstants = StokedUp.Constants.Constants;
using StokedUp.Models;

namespace StokedUp.Data
{
    public class LocalDatabase
    {
        private SQLiteAsyncConnection? _database;

        private readonly SemaphoreSlim _initializationLock = new(1, 1);

        private static readonly string[] AvailableCategories =
        {
            "Fresh Produce",
            "Dairy",
            "Meat",
            "Canned",
            "Frozen",
            "Drink",
            "Grain",
            "Sweets",
            "Oil",
            "Nut",
            "Condiment",
            "Miscellaneous"
        };

        //initialize database and create tables
        private async Task InitializeAsync()
        {
            if (_database != null)
                return;

            await _initializationLock.WaitAsync();

            try
            {
                if (_database != null)
                    return;

                //create SQLite connection
                var connection = new SQLiteAsyncConnection(
                    AppConstants.DatabasePath,
                    AppConstants.Flags);

                await connection.CreateTableAsync<StokpileItem>();
                await connection.CreateTableAsync<Category>();
                await AddAvailableCategoriesAsync(connection);

                _database = connection;
            }
            finally
            {  
                _initializationLock.Release(); 
            }
        }

        //add available categories if they do not already exist
        private static async Task AddAvailableCategoriesAsync(SQLiteAsyncConnection connection)
        {
            var existingCategories =
                await connection.Table<Category>().ToListAsync();

            foreach (string categoryType in AvailableCategories)
            {
                bool categoryExists = existingCategories.Any(
                    category => category.CategoryType == categoryType);

                if (!categoryExists)
                {
                    await connection.InsertAsync(
                        new Category {CategoryType = categoryType});
                }
            }
        }

        //retrieve available categories
        public async Task<List<Category>>GetCategoriesAsync()
        {
            await InitializeAsync();

            return await _database!.Table<Category>()
                .ToListAsync();
        }

        //retrieve all grocery items from Stokpile
        public async Task<List<StokpileItem>> GetStokpileItemsAsync()
        {
            await InitializeAsync();
            return await _database!.Table<StokpileItem>()
                .ToListAsync();
        }

        //US-01: Add Grocery Items to Stokpile
        public async Task AddStokpileItemAsync(StokpileItem item)
        {
            await InitializeAsync();
            await _database!.InsertAsync(item);
        }

        //US-03: Delete Grocery Items from Stokpile
        public async Task DeleteStokpileItemAsync(int id)
        {
            await InitializeAsync();
            await _database!.DeleteAsync<StokpileItem> (id);
        }
    }
}
