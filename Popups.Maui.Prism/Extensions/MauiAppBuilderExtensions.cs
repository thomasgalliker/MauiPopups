using Microsoft.Maui.LifecycleEvents;

namespace Popups.Maui.Prism
{
    public static class MauiAppBuilderExtensions
    {
        /// <summary>
        /// Configures Popups.Maui to use with this app.
        /// </summary>
        public static MauiAppBuilder UsePrismMauiPopups(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events =>
            {
#if IOS
                events.AddiOS(iOS => iOS.FinishedLaunching((application, launchOptions) =>
                {
                    return true;
                }));
#elif ANDROID
                events.AddAndroid(android => android.OnApplicationCreate(d =>
                {

                }));
                events.AddAndroid(android => android.OnCreate((activity, intent) =>
                {
                }));
                events.AddAndroid(android => android.OnNewIntent((activity, intent) =>
                {
                }));
#endif
            });

            // Service registrations
#if ANDROID || IOS
            //builder.Services.AddSingleton(c => ....);
            //builder.Services.AddSingleton<...,...>();
#endif

            return builder;
        }
    }
}