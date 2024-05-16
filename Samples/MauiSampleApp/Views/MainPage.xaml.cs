using Microsoft.Extensions.Logging;

namespace MauiSampleApp.Views
{
    public partial class MainPage : ContentPage, IDestructible
    {
        private readonly ILogger<MainPage> logger;

        public MainPage(ILogger<MainPage> logger)
        {
            this.InitializeComponent();

            this.logger = logger;
        }

        public void Destroy()
        {
            this.logger.LogDebug("Destroy");
        }
    }
}