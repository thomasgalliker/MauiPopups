using Popups.Maui.Prism.Behaviors;
using Popups.Maui.Prism.Dialogs;
using Prism.Behaviors;

namespace Popups.Maui.Prism
{
    public static class MauiAppBuilderExtensions
    {
        /// <summary>
        /// Configures Popups.Maui.Prism to use with this app.
        /// </summary>
        public static PrismAppBuilder UseMauiPopups(this PrismAppBuilder builder)
        {
            builder.MauiBuilder.UseMauiPopups();

            builder.RegisterTypes(RegisterTypes);

            return builder;
        }

        private static void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterScoped<INavigationService, PopupPageNavigationService>();
            containerRegistry.RegisterSingleton<IDialogService, PopupDialogService>();
            containerRegistry.RegisterSingleton<IPageBehaviorFactory, PopupPageBehaviorFactory>();
            containerRegistry.RegisterSingleton<IPageDialogService, PopupPageDialogService>();
        }
    }
}