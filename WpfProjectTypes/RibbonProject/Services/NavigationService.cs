﻿using System;
using System.Windows.Controls;
using System.Windows.Navigation;
using RibbonProject.Contracts.Services;
using RibbonProject.Contracts.ViewModels;
using RibbonProject.Helpers;

namespace RibbonProject.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IPageService _pageService;
        private Frame _frame;
        private object _lastParameterUsed;

        public event EventHandler<string> Navigated;

        public bool CanGoBack => _frame.CanGoBack;

        public NavigationService(IPageService pageService)
        {
            _pageService = pageService;
        }

        public void Initialize(Frame shellFrame)
        {
            if (_frame == null)
            {
                _frame = shellFrame;
                _frame.Navigated += OnNavigated;
            }
        }

        public void GoBack()
            => _frame.GoBack();

        public bool Navigate(string pageKey, object parameter = null, bool clearNavigation = false)
        {
            var pageType = _pageService.GetPageType(pageKey);

            if (_frame.Content?.GetType() != pageType || (parameter != null && !parameter.Equals(_lastParameterUsed)))
            {
                var dataContext = _frame.GetDataContext();
                if (dataContext is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatingFrom();
                }

                _frame.Tag = clearNavigation;
                var page = _pageService.GetPage(pageKey);
                var navigated = _frame.Navigate(page, parameter);
                if (navigated)
                {
                    _lastParameterUsed = parameter;
                }

                return navigated;
            }

            return false;
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (sender is Frame frame)
            {
                bool clearNavigation = (bool)frame.Tag;
                if (clearNavigation)
                {
                    frame.CleanNavigation();
                }

                var dataContext = frame.GetDataContext();
                if (dataContext is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedTo(e.ExtraData);
                }

                Navigated?.Invoke(sender, dataContext.GetType().FullName);
            }
        }
    }
}
