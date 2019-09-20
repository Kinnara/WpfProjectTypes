﻿using System.Threading;
using System.Threading.Tasks;
using MenuBarProject.Contracts.Services;
using MenuBarProject.Contracts.Views;
using MenuBarProject.ViewModels;
using Microsoft.Extensions.Hosting;

namespace MenuBarProject.Services
{
    internal class ApplicationHostService : IHostedService
    {
        private IThemeSelectorService _themeSelectorService;
        private IPersistAndRestoreService _persistAndRestoreService;
        private INavigationService _navigationService;
        private IShellWindow _shellWindow;

        public ApplicationHostService(IThemeSelectorService themeSelectorService, IPersistAndRestoreService persistAndRestoreService, INavigationService navigationService, IMenuNavigationService menuNavigationService, IShellWindow shellWindow)
        {
            _themeSelectorService = themeSelectorService;
            _persistAndRestoreService = persistAndRestoreService;
            _navigationService = navigationService;
            _shellWindow = shellWindow;
            menuNavigationService.Initialize(shellWindow);
            _navigationService.Initialize(_shellWindow.GetNavigationFrame());
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Initialize services that you need before app activation
            await InitializeAsync();

            _shellWindow.ShowWindow();
            _navigationService.Navigate(typeof(MainViewModel).FullName);

            // Tasks after activation
            await StartupAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            _persistAndRestoreService.PersistData();
        }

        private async Task InitializeAsync()
        {
            await Task.CompletedTask;
            _persistAndRestoreService.RestoreData();
            _themeSelectorService.SetTheme();
        }

        private async Task StartupAsync()
        {
            await Task.CompletedTask;
        }
    }
}
