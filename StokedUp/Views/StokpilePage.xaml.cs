using StokedUp.ViewModels;

namespace StokedUp.Views;

public partial class StokpilePage : ContentPage
{
    public StokpilePage()
    {
        InitializeComponent();
        BindingContext = new StokpilePageViewModel();
    }
}
