using StokedUp.Views;
namespace StokedUp;

public sealed partial class App : Application
{
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new StokpilePage());
    }
}
