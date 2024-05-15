using Microsoft.Maui.LifecycleEvents;
using Mopups.Hosting;
using Mopups.Pages;
#if IOS
using Mopups.Platforms.iOS;
#endif

namespace Popups.Maui
{
    public static class MauiAppBuilderExtensions
    {
        /// <summary>
        /// Configures Popups.Maui to use with this app.
        /// </summary>
        public static MauiAppBuilder UseMauiPopups(this MauiAppBuilder builder)
        {
            //builder.ConfigureMopups();

            builder.ConfigureLifecycleEvents(delegate (ILifecycleBuilder lifecycle)
            {
#if ANDROID        
                lifecycle.AddAndroid(delegate (IAndroidLifecycleBuilder d)
                {
                    d.OnBackPressed((Android.App.Activity activity) =>
                    {
                        return Mopups.Droid.Implementation.AndroidMopups.SendBackPressed();
                    });
                });
#endif
            }).ConfigureMauiHandlers(delegate (IMauiHandlersCollection handlers)
            {
#if ANDROID || IOS
                handlers.AddHandler(typeof(PopupPage), typeof(PopupPageHandler));
#endif
            });

            return builder;
        }
    }
}