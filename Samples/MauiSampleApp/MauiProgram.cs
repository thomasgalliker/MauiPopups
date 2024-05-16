using System.Diagnostics;
using CommunityToolkit.Maui;
using MauiSampleApp.ViewModels;
using MauiSampleApp.Views;
using Microsoft.Extensions.Logging;
using Popups.Maui;
using Popups.Maui.Prism;

namespace MauiSampleApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UsePrism(prism =>
                {
                    prism
                        .UseMauiPopups()
                        .RegisterTypes(RegisterTypes)
                        .CreateWindow(async (containerProvider, navigationService) =>
                        {
                            var result = await navigationService.NavigateAsync($"/{App.Pages.NavigationPage}/{App.Pages.MainPage}");
                            //var result = await navigationService.NavigateAsync($"{App.Pages.MainPage}");
                            if (!result.Success)
                            {
                                Debugger.Break();
                            }
                        });
                })
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddLogging(b =>
            {
                b.ClearProviders();
                b.SetMinimumLevel(LogLevel.Trace);
                b.AddDebug();
            });

            return builder.Build();
        }

        private static void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register services
            containerRegistry.RegisterSingleton<IPreferences>(() => Preferences.Default);

            // Register pages
            containerRegistry.RegisterForNavigation<MainPage, MainViewModel>(App.Pages.MainPage);
            containerRegistry.RegisterForNavigation<DetailPage, DetailViewModel>(App.Pages.DetailPage);
            containerRegistry.RegisterForNavigation<ContextMenuPopupPage, ContextMenuPopupViewModel>(App.Pages.ContextMenuPopupPage);
        }
    }
}