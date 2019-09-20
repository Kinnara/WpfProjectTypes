﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using MenuBarProject.Contracts.Services;
using MenuBarProject.Contracts.ViewModels;
using MenuBarProject.Models;
using MenuBarProject.ViewModels;
using MenuBarProject.Views;

namespace MenuBarProject.Services
{
    public class NavigationService : INavigationService
    {
        private IServiceProvider _serviceProvider;
        private Frame _frame;
        private object _lastExtraDataUsed;
        private readonly Dictionary<string, Type> _pages = new Dictionary<string, Type>();

        public event EventHandler<string> Navigated;

        public bool CanGoBack => _frame.CanGoBack;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Initialize(Frame shellFrame)
        {
            if (_frame == null)
            {
                _frame = shellFrame;
                _frame.Navigated += OnNavigated;
                _frame.NavigationFailed += OnNavigationFailed;
            }

            Configure(typeof(MainViewModel).FullName, typeof(MainPage));
            Configure(typeof(BlankViewModel).FullName, typeof(BlankPage));
            Configure(typeof(SettingsViewModel).FullName, typeof(SettingsPage));
        }

        private void Configure(string key, Type pageType)
        {
            lock (_pages)
            {
                if (_pages.ContainsKey(key))
                {
                    throw new ArgumentException($"The key {key} is already configured in NavigationService");
                }

                if (_pages.Any(p => p.Value == pageType))
                {
                    throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == pageType).Key}");
                }

                _pages.Add(key, pageType);
            }
        }

        public void GoBack()
            => _frame.GoBack();

        public bool Navigate(string pageKey, object extraData = null, bool clearNavigation = false)
        {
            var pageType = GetPageType(pageKey);
            if (_frame.Content?.GetType() != pageType || (extraData != null && !extraData.Equals(_lastExtraDataUsed)))
            {
                var page = _serviceProvider.GetService(pageType);
                if (_frame.Content is FrameworkElement element)
                {
                    if (element.DataContext is INavigationAware navigationAware)
                    {
                        navigationAware.OnNavigatingFrom();
                    }
                }
                _frame.Tag = clearNavigation;
                var navigated = _frame.Navigate(page, extraData);
                if (navigated)
                {
                    _lastExtraDataUsed = extraData;
                }

                return navigated;
            }

            return false;
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is FrameworkElement element)
            {
                if (element.DataContext is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedTo(e.ExtraData);
                }

                Navigated?.Invoke(sender, element.DataContext.GetType().FullName);
            }

            if (sender is Frame frame)
            {
                bool clearNavigation = (bool)frame.Tag;
                if (clearNavigation)
                {
                    do
                        frame.RemoveBackEntry();
                    while (frame.CanGoBack);
                }
            }
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
        }

        public Type GetPageType(string viewModelName)
        {
            Type pageType;
            lock (_pages)
            {
                if (!_pages.TryGetValue(viewModelName, out pageType))
                {
                    throw new ArgumentException($"Page not found: {viewModelName}. Did you forget to call NavigationService.Configure?");
                }
            }

            return pageType;
        }
    }
}
