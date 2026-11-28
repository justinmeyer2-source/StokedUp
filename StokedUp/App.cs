namespace StokedUp;

public sealed class App : Application
{
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new ContentPage
        {
            Title = "StokedUp",
            Content = new Label
            {
                Text = "StokedUp is running!",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 24
            }
        });
    }
}