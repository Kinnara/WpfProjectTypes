﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;
using RibbonProject.Contracts.Services;
using RibbonProject.Contracts.ViewModels;
using RibbonProject.Helpers;
using RibbonProject.Models;

namespace RibbonProject.ViewModels
{
    public class SettingsViewModel : Observable, INavigationAware
    {
        private AppTheme _theme;
        private string _versionDescription;
        private IThemeSelectorService _themeSelectorService;
        private ICommand _setThemeCommand;
        private ICommand _privacyStatementCommand;

        public ICommand SetThemeCommand => _setThemeCommand ?? (_setThemeCommand = new RelayCommand<string>(OnSetTheme));
        public ICommand PrivacyStatementCommand => _privacyStatementCommand ?? (_privacyStatementCommand = new RelayCommand(OnPrivacyStatement));

        public AppTheme Theme
        {
            get { return _theme; }
            set { Set(ref _theme, value); }
        }

        public string VersionDescription
        {
            get { return _versionDescription; }
            set { Set(ref _versionDescription, value); }
        }

        public SettingsViewModel(IThemeSelectorService themeSelectorService)
        {
            _themeSelectorService = themeSelectorService;
        }

        public void OnNavigatedTo(object parameter)
        {
            VersionDescription = GetVersionDescription();
            Theme = _themeSelectorService.GetCurrentTheme();
        }

        public void OnNavigatingFrom()
        {
        }

        private string GetVersionDescription()
        {
            var appName = "MyDotNetCoreWpfAppRibbon";
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var versionInfo = FileVersionInfo.GetVersionInfo(assemblyLocation);
            return $"{appName} - {versionInfo.FileVersion}";
        }

        private void OnSetTheme(string themeName)
        {
            var theme = (AppTheme)Enum.Parse(typeof(AppTheme), themeName);
            _themeSelectorService.SetTheme(theme);
        }

        private void OnPrivacyStatement()
        {
            // There is an open Issue on this
            // https://github.com/dotnet/corefx/issues/10361
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "https://YourPrivacyUrlGoesHere/",
                UseShellExecute = true
            };
            Process.Start(psi);
        }
    }
}
