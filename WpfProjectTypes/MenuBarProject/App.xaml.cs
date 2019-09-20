﻿using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using CoreProject.Contracts.Services;
using CoreProject.Services;
using MenuBarProject.Contracts.Services;
using MenuBarProject.Contracts.Views;
using MenuBarProject.Models;
using MenuBarProject.Services;
using MenuBarProject.ViewModels;
using MenuBarProject.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MenuBarProject
{
    //For more inforation about application lifecyle events see https://docs.microsoft.com/dotnet/framework/wpf/app-development/application-management-overview
    public partial class App : Application
    {
        private IHost _host;

        public App()
        {
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            //TODO WTS: Register your services, viewmodels and pages here

            //App Host
            services.AddHostedService<ApplicationHostService>();

            // Services
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IMenuNavigationService, MenuNavigationService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<IFilesService, FilesService>();
            services.AddSingleton<IPersistAndRestoreService, PersistAndRestoreService>();

            // Views and ViewModels
            services.AddTransient<IShellWindow, ShellWindow>();
            services.AddTransient<ShellWindowViewModel>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();

            services.AddTransient<BlankViewModel>();
            services.AddTransient<BlankPage>();

            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();

            //Configuration
            services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            // For more information about .NET generic host see  https://docs.microsoft.com/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0
            _host = Host.CreateDefaultBuilder(e.Args)
                    .ConfigureAppConfiguration(c => c.SetBasePath(appLocation))
                    .ConfigureServices(ConfigureServices)
                    .Build();

            await _host.StartAsync();
        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            _host = null;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // TODO WTS: Please handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0 

            // e.Handled = true;
        }
    }
}
