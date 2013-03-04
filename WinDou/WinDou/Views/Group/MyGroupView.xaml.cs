using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WinDou.ViewModels;
using System.Net.Browser;
using WinDou.Controls;

namespace WinDou.Views
{
    public partial class MyGroupView : WinDouAppPage
    {
        public MyGroupView()
        {
            InitializeComponent();
            this.DataContext = App.MyGroupViewModel;
        }

        void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            string cookie = webBrowser.InvokeScript("eval", "document.cookie") as string;
            if (e.Uri.Authority == "www.douban.com" && !string.IsNullOrEmpty(cookie) && cookie.IndexOf("bid") >= 0)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        CookieCollection cc = webBrowser.GetCookies();
                        foreach (Cookie c in cc)
                        {
                            if (c.Name == "dbcl2")
                            {
                                App.DoubanService.DoubanCookie = c.Value.Replace("\"", "");
                                webBrowser.Visibility = System.Windows.Visibility.Collapsed;
                                LoadTopics();
                                break;
                            }
                        }
                    });
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //获取cookie
            if (string.IsNullOrEmpty(App.DoubanService.DoubanCookie))
            {
                //webBrowser.ClearCookiesAsync();
                pivotContainer.Visibility = System.Windows.Visibility.Collapsed;
                webBrowser.Visibility = System.Windows.Visibility.Visible;
                webBrowser.Navigated += webBrowser_Navigated;
                webBrowser.Navigate(new Uri("http://www.douban.com/accounts/login"));
                
            }
            else if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                LoadTopics();
            }
        }

        private void LoadTopics()
        {
            pivotContainer.Visibility = System.Windows.Visibility.Visible;
            ToggleListBoxBusyStyle(listAllTopic, true);
            ToggleListBoxBusyStyle(listCreateTopic, true);
            ToggleListBoxBusyStyle(listReplyTopic, true);


            App.MyGroupViewModel.GetAllTopicsCompleted += new EventHandler<DoubanSearchCompletedEventArgs>(GetAllTopicsCompleted);
            App.MyGroupViewModel.GetAllTopics();

            App.MyGroupViewModel.GetCreateTopicsCompleted += new EventHandler<DoubanSearchCompletedEventArgs>(GetCreateTopicsCompleted);
            App.MyGroupViewModel.GetCreateTopics();

            App.MyGroupViewModel.GetReplyTopicsCompleted += new EventHandler<DoubanSearchCompletedEventArgs>(GetReplyTopicsCompleted);
            App.MyGroupViewModel.GetReplyTopics();
        }

        private void GetReplyTopicsCompleted(object sender, DoubanSearchCompletedEventArgs e)
        {
            App.MyGroupViewModel.GetReplyTopicsCompleted -= GetReplyTopicsCompleted;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                ToggleListBoxBusyStyle(listReplyTopic, false);
            });
        }

        private void GetCreateTopicsCompleted(object sender, DoubanSearchCompletedEventArgs e)
        {
            App.MyGroupViewModel.GetCreateTopicsCompleted -= GetCreateTopicsCompleted;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                ToggleListBoxBusyStyle(listCreateTopic, false);
            });
        }

        private void GetAllTopicsCompleted(object sender, DoubanSearchCompletedEventArgs e)
        {
            App.MyGroupViewModel.GetAllTopicsCompleted -= GetAllTopicsCompleted;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                ToggleListBoxBusyStyle(listAllTopic, false);
            });
        }

        private void linkBtnGroupTopic_Click(object sender, RoutedEventArgs e)
        {
            string tid = ((HyperlinkButton)sender).CommandParameter.ToString();
            if (!string.IsNullOrEmpty(tid))
            {
                NavigationService.Navigate(new Uri("/Views/Group/GroupTopicView.xaml?tid=" + tid + "", UriKind.Relative));
            }
        }

        private void linkBtnGroup_Click(object sender, RoutedEventArgs e)
        {
            string uid = ((HyperlinkButton)sender).CommandParameter.ToString();
            if (!string.IsNullOrEmpty(uid))
            {
                NavigationService.Navigate(new Uri("/Views/Group/GroupView.xaml?uid=" + uid + "", UriKind.Relative));
            }
        }

        private void btnLoadMoreAll_Click(object sender, RoutedEventArgs e)
        {
            ToggleListBoxBusyStyle(listAllTopic, true);
            App.MyGroupViewModel.GetAllTopicsCompleted += new EventHandler<DoubanSearchCompletedEventArgs>(GetAllTopicsCompleted);
            App.MyGroupViewModel.GetAllTopics(true);
        }

        private void btnLoadMoreCreate_Click(object sender, RoutedEventArgs e)
        {
            ToggleListBoxBusyStyle(listCreateTopic, true);
            App.MyGroupViewModel.GetCreateTopicsCompleted += new EventHandler<DoubanSearchCompletedEventArgs>(GetCreateTopicsCompleted);
            App.MyGroupViewModel.GetCreateTopics(true);
        }

        private void btnLoadMoreReply_Click(object sender, RoutedEventArgs e)
        {
            ToggleListBoxBusyStyle(listReplyTopic, true);
            App.MyGroupViewModel.GetReplyTopicsCompleted += new EventHandler<DoubanSearchCompletedEventArgs>(GetReplyTopicsCompleted);
            App.MyGroupViewModel.GetReplyTopics(true);
        }

    }
}