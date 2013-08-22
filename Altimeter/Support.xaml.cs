using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;

namespace Altimeter
{
    public partial class Support : PhoneApplicationPage
    {
        public bool gpsFlag;
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        //public event EventHandler<ApplicationUnhandledExceptionEventArgs> UnhandledException;

        public Support()
        {
            //this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
            settingsCheck();
        }

        private void settingsCheck()
        {
            if (!settings.Contains("gpsflag"))
            {
                settings.Add("gpsflag", toggleSwitch1.IsChecked);
            }
        }

        private void toggleSwitch1_Checked(object sender, RoutedEventArgs e)
        {
            saveSetting(true);
        }

        private void toggleSwitch1_Unchecked(object sender, RoutedEventArgs e)
        {
            saveSetting(false);
        }

        private void saveSetting(bool flag)
        {
            settings["gpsflag"] = flag;
            settings.Save();
            gpsFlag = flag;
        }

        private void toggleSwitch1_Loaded(object sender, RoutedEventArgs e)
        {
            settings.TryGetValue<bool>("gpsflag", out gpsFlag);

            toggleSwitch1.IsChecked = gpsFlag;
        }

        //private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        //{
        //    if (System.Diagnostics.Debugger.IsAttached)
        //    {
        //        System.Diagnostics.Debugger.Break();
        //    }
        //    e.Handled = true;
        //}
    }
}