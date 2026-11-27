using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace StokedUp.Controls;

public partial class NavigationBar : ContentView
{
    public static readonly BindableProperty SelectedTabProperty =
        BindableProperty.Create(
            nameof(SelectedTab), 
            typeof(string), 
            typeof(NavigationBar), 
            defaultValue: "Stokpile",
            propertyChanged: OnSelectedTabChanged);

    public static readonly BindableProperty NavigateCommandProperty =
        BindableProperty.Create(
            nameof(NavigateCommand), 
            typeof(ICommand), 
            typeof(NavigationBar));

    public string SelectedTab
    {
        get => (string)GetValue(SelectedTabProperty);
        set => SetValue(SelectedTabProperty, value);
    }

    public ICommand NavigateCommand
    {
        get => (ICommand)GetValue(NavigateCommandProperty);
        set => SetValue(NavigateCommandProperty, value);
    }

    public NavigationBar()
    {
        InitializeComponent();

        UpdateSelectedState(SelectedTab);
    }

    private static void OnSelectedTabChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is NavigationBar navBar && newValue is string tab)
        {
            navBar.UpdateSelectedState(tab);
        }
    }

    private void UpdateSelectedState(string selected)
    {
        if (BtnStokpile == null || BtnShopping == null ||
            BtnExpirationAlerts == null || BtnSettings == null)
            return;

        // Reset all
        SetButtonState(BtnStokpile, LblStokpile, false);
        SetButtonState(BtnShopping, LblShoppingList, false);
        SetButtonState(BtnExpirationAlerts, LblExpirationAlerts, false);
        SetButtonState(BtnSettings, LblSettings, false);

        // Highlight selected tab/page
        switch (selected)
        {
            case "Stokpile":
                SetButtonState(BtnStokpile, LblStokpile, true);
                break;
            case "ShoppingList":
                SetButtonState(BtnShopping, LblShoppingList, true);
                break;
            case "ExpirationAlerts":
                SetButtonState(BtnExpirationAlerts, LblExpirationAlerts, true);
                break;
            case "Settings":
                SetButtonState(BtnSettings, LblSettings, true);
                break;
        }
    }

    private void SetButtonState(Border border, Label label, bool isSelected)
    {
        border.BackgroundColor = isSelected ? Colors.LightGray : Colors.Transparent;
        label.FontAttributes = isSelected ? FontAttributes.Bold : FontAttributes.None;
    }
}