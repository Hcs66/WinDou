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
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Shell;
using WinDou.ViewModels;

namespace WinDou.Views
{
    public partial class GroupTopicView : WinDouAppPage
    {
        public GroupTopicView()
        {
            InitializeComponent();
            this.DataContext = App.GroupTopicViewModel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New &&
                NavigationContext.QueryString.ContainsKey("tid"))
            {
                string tid = NavigationContext.QueryString["tid"];
                if (!string.IsNullOrEmpty(tid))
                {
                    base.SetProgressIndicator(true);
                    App.GroupTopicViewModel.GetGroupTopicCompleted += new EventHandler<DoubanSearchCompletedEventArgs>(GetGroupTopicCompleted);
                    App.GroupTopicViewModel.GetGroupTopic(tid);

                    ToggleListBoxBusyStyle(listGroupTopicReview, true);
                    App.GroupTopicViewModel.GetGroupTopicReviewListCompleted += new EventHandler<DoubanSearchCompletedEventArgs>(GetGroupTopicReviewListCompleted);
                    App.GroupTopicViewModel.GetGroupTopicReviews(tid);

                    ToggleListBoxBusyStyle(listGroupTopicImage, true);
                    App.GroupTopicViewModel.GetGroupTopicImageListCompleted += new EventHandler<DoubanSearchCompletedEventArgs>(GetGroupTopicImageListCompleted);  
                }
            }
        }

        private void GetGroupTopicReviewListCompleted(object sender, DoubanSearchCompletedEventArgs e)
        {
            App.GroupTopicViewModel.GetGroupTopicReviewListCompleted -= GetGroupTopicReviewListCompleted;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (e.Result != null)
                {
                    pivotItemGroupTopicReview.Header = "评论(" + int.Parse(e.Result.ToString()).ToString() + ")";
                }
                ToggleListBoxBusyStyle(listGroupTopicReview, false);
            });
        }

        private void GetGroupTopicImageListCompleted(object sender, DoubanSearchCompletedEventArgs e)
        {
            App.GroupTopicViewModel.GetGroupTopicImageListCompleted -= GetGroupTopicImageListCompleted;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                ToggleListBoxBusyStyle(listGroupTopicImage, false);
            });
        }

        void GetGroupTopicCompleted(object s, DoubanSearchCompletedEventArgs args)
        {
            App.GroupTopicViewModel.GetGroupTopicCompleted -= GetGroupTopicCompleted;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                base.SetProgressIndicator(false);
                if (args.IsSuccess)
                {
                    contentContainer.Visibility = Visibility.Visible;
                    contentContainer.IsEnabled = true;
                    foreach (var content in App.GroupTopicViewModel.TopicContentList)
                    {
                        TextBlock tb = new TextBlock();
                        //tb.Width = 445;
                        tb.TextWrapping = TextWrapping.Wrap;
                        tb.Foreground = new SolidColorBrush(Colors.Black);
                        tb.FontSize = (double)App.Current.Resources["PhoneFontSizeMedium"];
                        tb.Text = content;
                        spContent.Children.Add(tb);
                    }
                    contentContainer.ScrollToVerticalOffset(0);
                }
                else
                {
                    ToastPrompt toast = new ToastPrompt();
                    toast.Message = args.Message;
                    toast.Show();
                }
            }
            );

        }

        private void appbarMenuHome_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnLoadMore_Click(object sender, RoutedEventArgs e)
        {
            ToggleListBoxBusyStyle(listGroupTopicReview, true);
            App.GroupTopicViewModel.GetGroupTopicReviewListCompleted += new EventHandler<DoubanSearchCompletedEventArgs>(GetGroupTopicReviewListCompleted);
            App.GroupTopicViewModel.LoadMoreReviews();
        }

    }
}