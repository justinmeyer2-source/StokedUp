
using StokedUp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace StokedUp.Views;

public partial class StokpilePage : ContentPage
{
    private readonly StokpilePageViewModel _viewModel;

    public StokpilePage()
    {
        InitializeComponent();

        _viewModel = IPlatformApplication.Current!
            .Services.GetRequiredService<StokpilePageViewModel>();

        BindingContext = _viewModel;
    }

    // STOK-225: Load predefined categories when the page opens.
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.Categories.Count == 0)
        {
            await _viewModel.LoadCategoriesAsync();
        }
    }
}
