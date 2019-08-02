﻿using System.Threading.Tasks;
using NavigationViewProject.Services;
using NavigationViewProject.Views;

namespace NavigationViewProject.Activation
{
    internal class DefaultActivationHandler : IActivationHandler
    {
        private NavigationService _navigationService;
        private ShellWindow _shelWindow;

        public DefaultActivationHandler(NavigationService navigationService, ShellWindow shelWindow)
        {
            _navigationService = navigationService;
            _shelWindow = shelWindow;
        }

        public bool CanHandle(object args)
            => !_navigationService.IsNavigated();

        public async Task HandleAsync(object args)
        {
            await Task.CompletedTask;
            _shelWindow.Show();
            _navigationService.Navigate<MainPage>();
        }
    }
}
