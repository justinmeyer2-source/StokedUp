using StokedUp.Data;
using StokedUp.ViewModels;
using System.Diagnostics;
// using StokedUp.Views;

namespace StokedUp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        //register database as singleton service
        builder.Services.AddSingleton<LocalDatabase>();

        //register ViewModels
        builder.Services.AddTransient<StokpileViewModel>();
        builder.Services.AddTransient<AddStokpileItemViewModel>();
        builder.Services.AddTransient<StokpilePageViewModel>();
        // builder.Services.AddTransient<EditStokpileItemViewModel>();

        //register Views
        // builder.Services.AddTransient<StokpilePage>();
        // builder.Services.AddTransient<AddStokpileItemPage>();
        // builder.Services.AddTransient<EditStokpileItemPage>();

#if DEBUG
        Debug.WriteLine("StokedUp application is starting.");
#endif

        return builder.Build();
    }
}
