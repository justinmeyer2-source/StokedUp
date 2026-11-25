/* using SQLite;

namespace StokedUp.Data;

internal static class TestDatabase
{
    internal static Task CheckAsync()
    {
        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "stokedup.db3");

        return Task.Run(() =>
        {
            using var connection = new SQLiteConnection(databasePath);
            connection.ExecuteScalar<int>("SELECT 1");
        });
    }
} /*
