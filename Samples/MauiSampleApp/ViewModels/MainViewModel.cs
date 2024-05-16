using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace MauiSampleApp.ViewModels
{
    public class MainViewModel : BindableBase, INavigatedAware, IDestructible
    {
        private readonly ILogger logger;
        private readonly IPageDialogService pageDialogService;
        private readonly INavigationService navigationService;
        private readonly IPreferences preferences;
        private IAsyncRelayCommand showPopupCommand;
        private IAsyncRelayCommand navigateToPageCommand;
        private IAsyncRelayCommand goBackCommand;

        private bool useModalNavigation;

        public MainViewModel(
            ILogger<MainViewModel> logger,
            IPageDialogService pageDialogService,
            INavigationService navigationService,
            IPreferences preferences)
        {
            this.logger = logger;
            this.pageDialogService = pageDialogService;
            this.navigationService = navigationService;
            this.preferences = preferences;
        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                await this.InitializeAsync();
            }
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        private async Task InitializeAsync()
        {
            try
            {
                this.UseModalNavigation = this.preferences.Get("UseModalNavigation", false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "InitializeAsync failed with exception");
                await this.pageDialogService.DisplayAlertAsync("Error", "Initialization failed", "OK");
            }
        }

        public bool UseModalNavigation
        {
            get => this.useModalNavigation;
            set
            {
                if (this.SetProperty(ref this.useModalNavigation, value))
                {
                    this.preferences.Set("UseModalNavigation", value);
                }
            }
        }

        public ICommand NavigateToPageCommand => this.navigateToPageCommand ??= new AsyncRelayCommand<string>(this.NavigateToPageAsync);

        private async Task NavigateToPageAsync(string page)
        {
            try
            {
                var parameters = new NavigationParameters
                {
                    { "key1", "value1" },
                    { KnownNavigationParameters.UseModalNavigation, this.UseModalNavigation }
                };

                var result = await this.navigationService.NavigateAsync(page, parameters);
                if (!result.Success)
                {
                    await this.pageDialogService.DisplayAlertAsync("Error", $"NavigateAsync to page {page} failed: " + result.Exception.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "ShowPopupAsync failed with exception");
                await this.pageDialogService.DisplayAlertAsync("Error", "ShowPopupAsync failed with exception", "OK");
            }
        }

        public ICommand ShowPopupCommand => this.showPopupCommand ??= new AsyncRelayCommand(this.ShowPopupAsync);

        private async Task ShowPopupAsync()
        {
            try
            {
                var page = App.Pages.ContextMenuPopupPage;

                var parameters = new NavigationParameters
                {
                    { "key1", "value1" }
                };

                var result = await this.navigationService.NavigateAsync(page, parameters);
                if (!result.Success)
                {
                    await this.pageDialogService.DisplayAlertAsync("Error", $"NavigateAsync to page {page} failed: " + result.Exception.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "ShowPopupAsync failed with exception");
                await this.pageDialogService.DisplayAlertAsync("Error", "ShowPopupAsync failed with exception", "OK");
            }
        }

        public ICommand GoBackCommand => this.goBackCommand ??= new AsyncRelayCommand(this.GoBackAsync);

        private async Task GoBackAsync()
        {
            try
            {
                var parameters = new NavigationParameters
                {
                    { "key1", "value1" }
                };

                var result = await this.navigationService.GoBackAsync(parameters);
                if (!result.Success)
                {
                    await this.pageDialogService.DisplayAlertAsync("Error", $"GoBackAsync failed: " + result.Exception.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "GoBackAsync failed with exception");
                await this.pageDialogService.DisplayAlertAsync("Error", "GoBackAsync failed with exception", "OK");
            }
        }

        public void Destroy()
        {
            this.logger.LogDebug("Destroy");
        }
    }
}
