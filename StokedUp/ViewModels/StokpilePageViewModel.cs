
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using StokedUp.Models;
using StokedUp.Data;

namespace StokedUp.ViewModels;

public class StokpilePageViewModel : INotifyPropertyChanged
{
    private readonly LocalDatabase _database;

    // STOK-226: Store the currently selected category.
    private string? _selectedCategory;

    // STOK-226: Handle category selection from the UI.
    public ICommand ToggleCategoryCommand { get; }

    // Existing database service.
    public StokpilePageViewModel(LocalDatabase database)
    {
        _database = database;

        ToggleCategoryCommand =
            new Command<CategoryFilter>(ToggleCategory);
    }

    // STOK-225: Store predefined categories for filtering.
    public ObservableCollection<CategoryFilter> Categories
        { get; } = new();

    // STOK-225: Retrieve predefined categories from SQLite.
    public async Task LoadCategoriesAsync()
    {
        var categories = await _database.GetCategoriesAsync();

        Categories.Clear();

        foreach (var category in categories)
        {
            Categories.Add(new CategoryFilter
            {
                Name = category.CategoryType,
                IsSelected = false
            });
        }
    }

    // STOK-226: Identify and update the selected category.
    private void ToggleCategory(CategoryFilter? category)
    {
        if (category == null)
        return;

        // STOK-228: Clear the selection when tapped again.
        if (category.IsSelected)
        {
            _selectedCategory = null;
        }
        else
        {
            _selectedCategory = category.Name;
        }

    // Update the visual selection of every category.
        foreach (var availableCategory in Categories)
        {
            availableCategory.IsSelected =
                availableCategory.Name == _selectedCategory;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}