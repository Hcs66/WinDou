
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
using System.Windows.Navigation;

namespace WinDou.Views
{
    public class EntityEditingPage : WinDouAppPage
    {

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // If the the user made changes to an entity on the page we can check for them on the datacontext
            // and warn the user when navigating away from the page
            if (e.NavigationMode == NavigationMode.Back)
            {

            }

            base.OnNavigatingFrom(e);
        }
    }
}
