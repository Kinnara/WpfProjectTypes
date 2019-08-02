﻿using System.Collections.Generic;
using System.Windows;
using BlankProject.Services;

namespace BlankProject
{
    public static class ApplicationExtensions
    {
        private const string _appPropertiesFile = "AppProperties.txt";

        public static void RestoreProperties(this Application app)
        {
            var lines = IsolatedStorageService.ReadLines(_appPropertiesFile);
            foreach (var line in lines)
            {
                var settingPairValue = line.Split(',');
                app.Properties[settingPairValue[0]] = settingPairValue[1];
            }
        }

        public static void SaveProperties(this Application app)
        {
            var lines = new List<string>();
            foreach (var key in app.Properties.Keys)
            {
                lines.Add($"{key},{app.Properties[key]}");
            }

            IsolatedStorageService.SaveLines(_appPropertiesFile, lines);
        }
    }
}
