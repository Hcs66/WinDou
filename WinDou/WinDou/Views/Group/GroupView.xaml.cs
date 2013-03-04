using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WinDou.ViewModels;

namespace WinDou.Views
{
    public partial class GroupView : WinDouAppPage
    {
        public GroupView()
        {
            InitializeComponent();
            this.DataContext = App.GroupViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New 
                && NavigationContext.QueryString.ContainsKey("uid"))
            {
                string uid = NavigationContext.QueryString["uid"];
                base.SetProgressIndicator(true);
                //加载小组信息
                App.GroupViewModel.GetGroupCompleted += GetGroupCompleted;
                App.GroupViewModel.GetGroup(uid);
                //加载小组话题
                ToggleListBoxBusyStyle(listGroupTopicReview, true);
                App.GroupViewModel.GetGroupTopicsCompleted += new EventHandler<DoubanSearchCompletedEventArgs>(GetGroupTopicsCompleted);
                App.GroupViewModel.GetGroupTopics(uid);
            }
        }

        private void GetGroupTopicsCompleted(object sender, DoubanSearchCompletedEventArgs e)
        {
            App.GroupViewModel.GetGroupTopicsCompleted -= GetGroupTopicsCompleted;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                ToggleListBoxBusyStyle(listGroupTopicReview, false);
            });
        }

        private void GetGroupCompleted(object sender, EventArgs e)
        {
            App.GroupViewModel.GetGroupCompleted -= GetGroupCompleted;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                this.SetProgressIndicator(false);
                StackPanelGroup.Visibility = System.Windows.Visibility.Visible;
                foreach (var content in App.GroupViewModel.GroupContentList)
                {
                    TextBlock tb = new TextBlock();
                    //tb.Width = 445;
                    tb.TextWrapping = TextWrapping.Wrap;
                    tb.Foreground = new SolidColorBrush(Colors.Black);
                    tb.FontSize = (double)App.Current.Resources["PhoneFontSizeMedium"];
                    tb.Text = content;
                    spContent.Children.Add(tb);
                }
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

        private void btnLoadMore_Click(object sender, RoutedEventArgs e)
        {
            ToggleListBoxBusyStyle(listGroupTopicReview,true);
            App.GroupViewModel.GetGroupTopicsCompleted += new EventHandler<DoubanSearchCompletedEventArgs>(GetGroupTopicsCompleted);
            App.GroupViewModel.GetGroupTopics(App.GroupViewModel.CurrentGroup.Id,true);
        }
    }
}