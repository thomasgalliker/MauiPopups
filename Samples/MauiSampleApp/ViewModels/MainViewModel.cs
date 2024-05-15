using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace MauiSampleApp.ViewModels
{
    public class MainViewModel : BindableBase, INavigatedAware
    {
        private readonly ILogger logger;
        private readonly IPageDialogService pageDialogService;
        private readonly IDialogService dialogService;
        private readonly INavigationService navigationService;
        private readonly IShare share;
        private readonly IPreferences preferences;

        private IAsyncRelayCommand showPopupCommand;

        public MainViewModel(
            ILogger<MainViewModel> logger,
            IPageDialogService pageDialogService,
            IDialogService dialogService,
            INavigationService navigationService,
            IShare share,
            IPreferences preferences)
        {
            this.logger = logger;
            this.pageDialogService = pageDialogService;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.share = share;
            this.preferences = preferences;
        }

        public async void OnNavigatedFrom(INavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                await this.InitializeAsync();
            }
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        private async Task InitializeAsync()
        {
            try
            {
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "InitializeAsync failed with exception");
                await this.pageDialogService.DisplayAlertAsync("Error", "Initialization failed", "OK");
            }
        }

        public ICommand ShowPopupCommand => this.showPopupCommand ??= new AsyncRelayCommand(this.ShowPopupAsync);

        private async Task ShowPopupAsync()
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "ShowPopupAsync failed with exception");
                await this.pageDialogService.DisplayAlertAsync("Error", "ShowPopupAsync failed with exception", "OK");
            }
        }
    }
}
