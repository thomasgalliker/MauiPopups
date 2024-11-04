using Microsoft.Extensions.DependencyInjection.Extensions;
using Mopups.Hosting;
using Mopups.Services;

namespace Popups.Maui
{
    public static class MauiAppBuilderExtensions
    {
        /// <summary>
        /// Configures Popups.Maui to use with this app.
        /// </summary>
        public static MauiAppBuilder UseMauiPopups(this MauiAppBuilder builder)
        {
            builder.ConfigureMopups();
            builder.Services.TryAddSingleton(MopupService.Instance);

            return builder;
        }
    }
}