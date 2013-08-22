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
using System.Device.Location;
using Microsoft.Phone.Controls.Maps.Platform;
using Microsoft.Phone.Controls.Maps;
using System.IO.IsolatedStorage;

namespace Altimeter
{
    public partial class MainPage : PhoneApplicationPage
    {
        public GeoCoordinateWatcher watcher;
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        bool gpsFlag;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            ResetMap();
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            InitializeWatcher();
        }

        private void InitializeWatcher()
        {
            if (!settings.Contains("gpsflag"))
            {
                settings.Add("gpsflag", false);
            }


            // Reinitialize the GeoCoordinateWatcher 
            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            watcher.MovementThreshold = 100;//distance in metres 

            // Add event handlers for StatusChanged and PositionChanged events 
            watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
            watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);


            settings.TryGetValue<bool>("gpsflag", out gpsFlag);

            if (gpsFlag)
            {
                // Start data acquisition 
                watcher.Start();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("This app need your location data to track your location and altitude.", "GPS Permission request", MessageBoxButton.OKCancel);
                if (result.Equals(MessageBoxResult.OK))
                {
                    // Start data acquisition 
                    watcher.Start();

                    gpsFlag = true;
                    settings["gpsflag"] = gpsFlag;
                }
                else
                {
                    tbStatus.Text = "GPS data: OFF";
                }
            }
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e) 
        { 
            Deployment.Current.Dispatcher.BeginInvoke(() => MyStatusChanged(e)); 
     
        } 

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e) 
        { 
            Deployment.Current.Dispatcher.BeginInvoke(() => MyPositionChanged(e)); 
        }

        void MyPositionChanged(GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            // Update the map to show the current location
            Location ppLoc = new Location();
            ppLoc.Latitude = e.Position.Location.Latitude;
            ppLoc.Longitude = e.Position.Location.Longitude;          
            mapMain.SetView(ppLoc, 15.0);

            //update pushpin location and show
            MapLayer.SetPosition(ppLocation, ppLoc);
            ppLocation.Visibility = System.Windows.Visibility.Visible;
            if (e.Position.Location.Altitude.Equals(0.0))
            {
                tbStatus.Text = "finding altitude...";
                tbAltitude.Text = "?m. (?ft.)";
            }
            else
            {
                tbAltitude.Text = string.Format("{0:0.00} ", e.Position.Location.Altitude) + "m. (" + string.Format("{0:0.00} ", e.Position.Location.Altitude * 3.2808399) + " ft.)";
            }
        }

        void MyStatusChanged(GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    // The location service is disabled or unsupported.
                    // Alert the user
                    tbStatus.Text = "GPS data disabled";
                    break;
                case GeoPositionStatus.Initializing:
                    // The location service is initializing.
                    // Disable the Start Location button
                    tbStatus.Text = "looking for you…";
                    break;
                case GeoPositionStatus.NoData:
                    // The location service is working, but it cannot get location data
                    // Alert the user and enable the Stop Location button
                    tbStatus.Text = "No GPS data";

                    break;
                case GeoPositionStatus.Ready:
                    // The location service is working and is receiving location data
                    // Show the current position and enable the Stop Location button
                    tbStatus.Text = "Ready!";
                    break;
            }
        }

        void ResetMap()
        {
            Location ppLoc = new Location();
            ppLoc.Latitude = 0.0;
            ppLoc.Longitude = 0.0;
            mapMain.SetView(ppLoc, 1.0);

            //update pushpin location and show
            MapLayer.SetPosition(ppLocation, ppLoc);
            ppLocation.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void about_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Support.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeWatcher();
        }

    }
}