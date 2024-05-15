using Foundation;
using Microsoft.Extensions.Logging;
using UIKit;

namespace MauiSampleApp
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp()
        {
            return MauiProgram.CreateMauiApp();
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            TaskScheduler.UnobservedTaskException += this.TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;

            return base.FinishedLaunching(application, launchOptions);
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var logger = IPlatformApplication.Current.Services.GetRequiredService<ILogger<AppDelegate>>();
            logger.LogError(e.Exception, "TaskScheduler_UnobservedTaskException");
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var logger = IPlatformApplication.Current.Services.GetRequiredService<ILogger<AppDelegate>>();
            logger.LogError(e.ExceptionObject as Exception, "CurrentDomain_UnhandledException");
        }
    }
}