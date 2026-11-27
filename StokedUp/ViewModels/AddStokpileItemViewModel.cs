using IntelliJ.Lang.Annotations;
using StokedUp.Data;
using StokedUp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace StokedUp.ViewModels
{
    internal class AddStokpileItemViewModel : INotifyPropertyChanged
    {
        private readonly LocalDatabase _database;

        public ObservableCollection<string> Categories { get; } = new();

        //commands
        public Command AddItemCommand { get; }

        //receive database service
        public AddStokpileItemViewModel(LocalDatabase localDatabase)
        {
            _database = localDatabase;

            AddItemCommand = new Command(async () => await AddItemAsync(),
                () => !IsBusy);
        }

        //grocery item form properties
        private string _itemName = string.Empty;
        public string ItemName
        {
            get => _itemName;
            set => SetProperty(ref _itemName, value);
        }

        private string _quantity = "1";
        public string Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        private string _itemUnit = string.Empty;

        public string ItemUnit
        {
            get => _itemUnit;
            set => SetProperty(ref _itemUnit, value);
        }

        private string? _selectedCategory;
        public string? SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        private DateTime _expirationDate = DateTime.Today;
        public DateTime ExpirationDate
        {
            get => _expirationDate;
            set => SetProperty(ref _expirationDate, value);
        }

        private string _notes = string.Empty;
        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    AddItemCommand.ChangeCanExecute();
                }
            }
        }

        //Load existing categories from SQLite
        public async Task LoadCategoriesAsync()
        {
            if (_isBusy)
                return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                Categories.Clear();

                var categories = await _database.GetCategoriesAsync();

                foreach (var category in categories)
                {
                    Categories.Add(category.CategoryType);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Unable to load grocery categories.";
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        //US-01: Add grocery items to Stokpile
        private async Task AddItemAsync()
        {
            if (IsBusy || !ValidateItem(out double quantity))
                return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var newItem = new StokpileItem
                {
                    Name = ItemName.Trim(),
                    Quantity = quantity,
                    Unit = ItemUnit.Trim(),
                    Category = SelectedCategory!,
                    ExpirationDate = ExpirationDate,
                    Notes = Notes.Trim()
                };

                //save item to SQLite
                await _database.AddStokpileItemAsync(newItem);

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
        private bool ValidateItem(out double quantity)
        {
            quantity = 0;
            ErrorMessage = string.Empty;

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

            if (!double.TryParse(Quantity, out quantity) ||
                double.IsNaN(quantity) || double.IsInfinity(quantity)
                || quantity <= 0)
            {
                ErrorMessage = "Quantity must be a valid number greater than 0.";
                return false;
            }

            if (ItemUnit.Length > 10)
            {
                ErrorMessage = "Unit cannot exceed 10 characters.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(SelectedCategory) ||
                    !Categories.Contains(SelectedCategory))
            {
                ErrorMessage = "Please select a valid category.";
                return false;
            }

            if (ExpirationDate.Date < DateTime.Today)
            {
                ErrorMessage = "Expiration date cannot be in the past.";
                return false;
            }

            if (Notes.Length > 500)
            {
                ErrorMessage = "Notes cannot exceed 500 characters.";
                return false;
            }

            return true;
        }

        //reset form after adding an item
        private void ClearForm()
        {
            ItemName = string.Empty;
            Quantity = "1";
            ItemUnit = string.Empty;
            SelectedCategory = null;
            ExpirationDate = DateTime.Today;
            Notes = string.Empty;
        }

        //update bound properties when values change
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
        //update UI when property changes
        protected bool SetProperty<T>(ref T backingField,
            T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;

            backingField = value;

            OnPropertyChanged(propertyName);

            return true;
        }
    }
}
