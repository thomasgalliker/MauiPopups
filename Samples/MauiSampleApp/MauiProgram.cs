using CommunityToolkit.Maui;
using MauiSampleApp.ViewModels;
using MauiSampleApp.Views;
using Microsoft.Extensions.Logging;
using Popups.Maui;

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
                        .RegisterTypes(RegisterTypes)
                        .CreateWindow(async (containerProvider, navigationService) =>
                        {
                            var result = await navigationService.NavigateAsync($"/{App.Pages.NavigationPage}/{App.Pages.MainPage}");
                            if (!result.Success)
                            {

                            }
                        });
                })
                .UseMauiPopups()
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
            containerRegistry.RegisterSingleton<IShare>(() => Share.Default);
            containerRegistry.RegisterSingleton<IPreferences>(() => Preferences.Default);
            containerRegistry.RegisterSingleton<IEmail>(() => Email.Default);
            containerRegistry.RegisterSingleton<IAppInfo>(() => AppInfo.Current);
            containerRegistry.RegisterSingleton<IDeviceInfo>(() => DeviceInfo.Current);
            containerRegistry.RegisterSingleton<IFileSystem>(() => FileSystem.Current);

            containerRegistry.RegisterForNavigation<MainPage, MainViewModel>(App.Pages.MainPage);
        }
    }
}