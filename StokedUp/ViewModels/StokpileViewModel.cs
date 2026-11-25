using Microsoft.Extensions.Options;
using StokedUp.Data;
using StokedUp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace StokedUp.ViewModels
{
    public class StokpileViewModel : INotifyPropertyChanged
    {
        //notifies UI when properties change
        public event PropertyChangedEventHandler? PropertyChanged;

        //grocery items displayed in Stokpile
        public ObservableCollection<StokpileItem> StokpileItems
                                                  { get; set; } = new();

        //commands connect to page buttons
        public ICommand AddItemCommand { get; }
        public ICommand DeleteItemCommand { get; }

        //grocery item form properties
        private string itemName = string.Empty;
        public string ItemName
        {
            get => itemName;
            set => SetProperty(ref itemName, value);
        }

        private double quantity = 1;
        public double Quantity
        {
            get => quantity;
            set => SetProperty(ref quantity, value);
        }

        private string itemUnit = string.Empty;

        public string ItemUnit
        {
            get => itemUnit;
            set => SetProperty(ref itemUnit, value);
        }

        public ObservableCollection<string> Categories { get; } = new();
       
        private string? selectedCategory;
        public string? SelectedCategory
        {
            get => selectedCategory;
            set => SetProperty(ref selectedCategory, value);
        }

        private DateTime expirationDate = DateTime.Today;
        public DateTime ExpirationDate
        {
            get => expirationDate;
            set => SetProperty(ref expirationDate, value);
        }

        private string notes = string.Empty;
        public string Notes
        {
            get => notes;
            set => SetProperty(ref notes, value);
        }

        //status of item properties
        private string errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                if (SetProperty(ref isBusy, value))
                {
                    UpdateCommandStates();
                }
            }
        }

        //initialize commands
        public StokpileViewModel()
        {
            AddItemCommand = new Command(async () => await AddItemAsync(),
                                                            () => !IsBusy);
            DeleteItemCommand = new Command<StokpileItem>(
                                async item => await DeleteItemAsync(item),
                                item => !IsBusy && item != null);
        }

        //Update UI when property changes
        private bool SetProperty<T>(ref T backingField,
            T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;

            backingField = value;

            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));

            return true;
        }

        //update command availability when busy state changes
        private void UpdateCommandStates()
        {
            (AddItemCommand as Command)?.ChangeCanExecute();
            (DeleteItemCommand as Command<StokpileItem>)?.ChangeCanExecute();
        }

        //Load existing Stokpile items and categories from SQLite
        public async Task LoadStokpileAsync()
        {
            if (isBusy) return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                await LocalDatabase.InitializeAsync();

                var categories = await LocalDatabase.GetCategoriesAsync();
                Categories.Clear();

                foreach (var category in categories)
                {
                    Categories.Add(category.CategoryType);
                }

                await UpdateStokpileAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Unable to load Stokpile items.";
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        //retrieve Stokpile items from SQLite and update Stokpile UI
        private async Task UpdateStokpileAsync()
        {
            var items = await LocalDatabase.GetStokpileItemsAsync();

            StokpileItems.Clear();

            foreach (var item in items)
            {
                StokpileItems.Add(item);
            }
        }

        //US-01: Add grocery items to Stokpile

        private async Task AddItemAsync()
        {
            if (IsBusy) return;

            if (!ValidateItem()) return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var newItem = new StokpileItem
                {
                    Name = ItemName.Trim(),
                    Quantity = Quantity,
                    Unit = ItemUnit.Trim(),
                    Category = SelectedCategory!,
                    ExpirationDate = ExpirationDate,
                    Notes = Notes.Trim()
                };

                //save item to SQLite
                await LocalDatabase.AddStokpileItemAsync(newItem);

                //update Stokpile UI
                await UpdateStokpileAsync();

                ClearForm();

                await Shell.Current.DisplayAlertAsync(
                    "Success", "Grocery item added to your Stokpile!", "OK");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Unable to add grocery item.";
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        //validate Stokpile item info before saving
        private bool ValidateItem()
        {
            if (string.IsNullOrWhiteSpace(ItemName))
            {
                ErrorMessage = "Please enter a grocery item name.";
                return false;
            }

            if (ItemName.Trim().Length > 100)
            {
                ErrorMessage = "Item name cannot exceed 100 characters.";
                return false;
            }

            if (double.IsNaN(Quantity) || double.IsInfinity(Quantity)
                || Quantity <= 0)
            {
                ErrorMessage = "Quantity must be a valid number greater than 0.";
                return false;
            }

            if (ItemUnit.Length > 20)
            {
                ErrorMessage = "Unit cannot exceed 20 characters.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(SelectedCategory) ||
                    !Categories.Contains(SelectedCategory))
            {
                ErrorMessage = "Please select a valid category.";
                return false;
            }

            if (Notes.Length > 500)
            {
                ErrorMessage = "Notes cannot exceed 500 characters.";
                return false;
            }

            return true;
        }


        //US-03: Delete grocery items from Stokpile
        private async Task DeleteItemAsync(StokpileItem? item)
        {
            if (item == null || IsBusy)
                return;

            bool confirm = await Shell.Current.DisplayAlertAsync(
                "Delete Stokpile Item",
                $"Are you sure you want to delete {item.Name} from your Stokpile?",
                "Delete", "Cancel");

            if (!confirm)
                return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                await LocalDatabase.DeleteStokpileItemAsync(item.Id);

                //remove item from the displayed Stokpile
                StokpileItems.Remove(item);

                await Shell.Current.DisplayAlertAsync(
                    "Deleted",
                    "Grocery item removed from your Stokpile.",
                    "OK");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Unable to delete grocery item.";
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        //reset form after adding an item
        private void ClearForm()
        {
            ItemName = string.Empty;
            Quantity = 1;
            ItemUnit = string.Empty;
            SelectedCategory = null;
            ExpirationDate = DateTime.Today;
            Notes = string.Empty;
        }
    }
}