using StokedUp.Data;
using StokedUp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StokedUp.ViewModels
{
    public class StokpileViewModel : INotifyPropertyChanged
    {
        private readonly LocalDatabase _database;

        //grocery items displayed in Stokpile
        public ObservableCollection<StokpileItem> StokpileItems { get; } = new();

        //commands
        public Command<StokpileItem> DeleteItemCommand { get; }

        //receive database service
        public StokpileViewModel(LocalDatabase localDatabase)
        {
            _database = localDatabase;

            DeleteItemCommand = new Command<StokpileItem>(
                async item => await DeleteItemAsync(item),
                item => !IsBusy && item != null);
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
                    DeleteItemCommand.ChangeCanExecute();
                }
            }
        }

        //Load existing Stokpile items from SQLite
        public async Task LoadStokpileAsync()
        {
            if (_isBusy) return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

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
            var items = await _database.GetStokpileItemsAsync();

            StokpileItems.Clear();

            foreach (var item in items)
            {
                StokpileItems.Add(item);
            }
        }

        //US-03: Delete grocery items from Stokpile
        private async Task DeleteItemAsync(StokpileItem? item)
        {
            if (item == null || IsBusy)
                return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                bool confirm = await Shell.Current.DisplayAlertAsync(
                           "Delete Stokpile Item",
                           $"Are you sure you want to delete {item.Name} from your Stokpile?",
                           "Delete",
                           "Cancel");

                if (!confirm)
                    return;

                await _database.DeleteStokpileItemAsync(item.Id);

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