﻿using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MauiSampleApp.Views;
using Microsoft.Extensions.Logging;

namespace MauiSampleApp.ViewModels
{
    public class ContextMenuPopupViewModel : BindableBase, INavigatedAware
    {
        private readonly ILogger logger;
        private readonly INavigationService navigationService;
        private readonly IPageDialogService pageDialogService;

        private IAsyncRelayCommand showPopupCommand;
        private IAsyncRelayCommand loginCommand;

        public ContextMenuPopupViewModel(
            ILogger<ContextMenuPopupViewModel> logger,
            INavigationService navigationService, IPageDialogService pageDialogService)
        {
            this.logger = logger;
            this.navigationService = navigationService;
            this.pageDialogService = pageDialogService;
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
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "InitializeAsync failed with exception");
            }
        }

        public ICommand GoBackCommand => this.showPopupCommand ??= new AsyncRelayCommand(this.GoBackAsync);

        public IAsyncRelayCommand LoginCommand =>
            this.loginCommand ??= new AsyncRelayCommand(() => this.navigationService.NavigateAsync(nameof(DetailPage)));

        private async Task GoBackAsync()
        {
            try
            {
                var close = await this.pageDialogService.DisplayAlertAsync("Confirmation", "Are you sure you want to close this?", "Yes",
                    "No").ConfigureAwait(true);
                if (!close)
                {
                    return;
                }

                var navigationParameters = new NavigationParameters { { "key2", "value2" } };

                var result = await this.navigationService.GoBackAsync(navigationParameters);
                if (!result.Success)
                {
                    Debugger.Break();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "ShowPopupAsync failed with exception");
            }
        }
    }
}