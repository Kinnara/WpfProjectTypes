﻿using System.Windows.Controls;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using MenuBarProject.Contracts.Services;
using MenuBarProject.Contracts.ViewModels;
using MenuBarProject.Helpers;

namespace MenuBarProject.Services
{
    public class RightPaneService : IRightPaneService
    {
        private readonly IPageService _pageService;
        private Frame _frame;
        private object _lastParameterUsed;
        private SplitView _splitView;

        public RightPaneService(IPageService pageService)
        {
            _pageService = pageService;
        }

        public void Initialize(Frame rightPaneFrame, SplitView splitView)
        {
            _frame = rightPaneFrame;
            _splitView = splitView;
            _frame.Navigated += OnNavigated;
        }

        public void OpenInRightPane(string pageKey, object parameter = null)
        {
            var pageType = _pageService.GetPageType(pageKey);
            if (_frame.Content?.GetType() != pageType || (parameter != null && !parameter.Equals(_lastParameterUsed)))
            {
                var dataContext = _frame.GetDataContext();
                if (dataContext is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatingFrom();
                }

                var page = _pageService.GetPage(pageKey);
                var navigated = _frame.Navigate(page, parameter);
                if (navigated)
                {
                    _lastParameterUsed = parameter;
                }
            }

            _splitView.IsPaneOpen = true;
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (sender is Frame frame)
            {
                frame.CleanNavigation();
                var dataContext = frame.GetDataContext();
                if (dataContext is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedTo(e.ExtraData);
                }
            }
        }
    }
}
