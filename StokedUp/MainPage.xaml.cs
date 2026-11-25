using StokedUp.Data;

namespace StokedUp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnCheckDatabaseClicked(object? sender, EventArgs e)
    {
        DatabaseCheckButton.IsEnabled = false;
        DatabaseStatus.Text = "Checking SQLite…";

        try
        {
            await TestDatabase.CheckAsync();
            DatabaseStatus.Text = "SQLite is ready.";
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(exception);
            DatabaseStatus.Text = "Could not open SQLite. See the debug output.";
        }
        finally
        {
            DatabaseCheckButton.IsEnabled = true;
        }
    }
}
