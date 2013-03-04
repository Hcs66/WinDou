using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Navigation;
using System.Collections.Generic;
using WinDou.Controls;

namespace WinDou.Views
{
    public class WinDouAppPage : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;

        protected List<EventHandler<EventArgs>> InitialDataHandlers { get; set; }

        public WinDouAppPage()
        {
           //Loaded += OnPageLoaded;
            InitialDataHandlers = new List<EventHandler<EventArgs>>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back && InitialDataHandlers.Count == 0)
            {
                //RestoreState(this, null);
            }

            base.OnNavigatedTo(e);
        }

        protected virtual void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            // Show the system tray if data is being loaded
            if (!App.IsAppDataLoaded)
            {
                if (!SystemTray.GetIsVisible(this))
                {
                    SystemTray.SetIsVisible(this, true);
                }
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
             //(Application.Current as App).InitialDataLoaded -= WinDouAppPage_InitialDataLoaded;
            //Loaded -= OnPageLoaded;

            //foreach (EventHandler<EventArgs> dataLoadedHandler in InitialDataHandlers)
            //{
            //     (Application.Current as App).InitialDataLoaded -= dataLoadedHandler;
            //}

            // Store the state unless "exiting" the page
            if (e.NavigationMode != NavigationMode.Back)
            {
                //StoreState();
            }

            base.OnNavigatedFrom(e);
        }

        void WinDouAppPage_InitialDataLoaded(object sender, EventArgs e)
        {
            SetProgressIndicator(false);
        }

        public void SetProgressIndicator(bool isVisible)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (null != progressIndicator)
                {
                    progressIndicator.IsVisible = isVisible;
                }
                else
                {
                    progressIndicator = new Microsoft.Phone.Shell.ProgressIndicator();
                    Microsoft.Phone.Shell.SystemTray.SetProgressIndicator(this, progressIndicator);
                    progressIndicator.Text = "处理中";
                    progressIndicator.IsIndeterminate = true;
                    progressIndicator.IsVisible = true;
                }
                SystemTray.SetIsVisible(this, isVisible);
            });
        }

        /// <summary>
        /// Checks whether or not the application's state indicates that the application has returned from tombstoning
        /// and has yet to load its data. If so, the supplied handlers are registered to the application's data loading event.
        /// </summary>
        /// <param name="args">Page's navigation arguments.</param>
        /// <param name="handlers">Handlers to register to the application's event which indicates all data has been
        /// loaded.</param>
        /// <returns>True if the application has indeed returned from tombstoning and false otherwise.</returns>
        public bool RegisterForInitialDataLoadCompleted(params EventHandler<EventArgs>[] handlers)
        {
            lock (App.SyncObject)
            {
                //TODO: Removing the App.WasTombstoned check.. 
                if (!App.IsAppDataLoaded /* && App.WasTombstoned*/ )
                {
                    foreach (EventHandler<EventArgs> handler in handlers)
                    {
                        InitialDataHandlers.Add(handler);
                        //(Application.Current as App).InitialDataLoaded += handler;
                    }

                    return true;
                }

                return false;
            }
        }

        protected void ToggleListBoxBusyStyle(ProgressLLS list, bool isBusy)
        {
            list.IsBusy = isBusy;
            //this.SetProgressIndicator(isBusy);
        }
    }
}
