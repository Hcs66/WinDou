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

namespace WinDou.Views
{
    public partial class SubjectReviewView : WinDouAppPage
    {
        public SubjectReviewView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New &&
                NavigationContext.QueryString.ContainsKey("reviewId"))
            {
                string reviewId = NavigationContext.QueryString["reviewId"];
                if (!string.IsNullOrEmpty(reviewId))
                {
                    base.SetProgressIndicator(true);
                    App.SubjectReviewViewModel.GetReviewCompleted += new EventHandler<ViewModels.DoubanSearchCompletedEventArgs>(GetReviewCompleted);
                    App.SubjectReviewViewModel.GetReview(reviewId);
                }
            }
        }

        void GetReviewCompleted(object s, ViewModels.DoubanSearchCompletedEventArgs args)
        {
            App.SubjectReviewViewModel.GetReviewCompleted -= GetReviewCompleted;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (args.IsSuccess)
                {
                    contentContainer.Visibility = Visibility.Visible;
                    contentContainer.IsEnabled = false;
                    DataContext = args.Result;
                    this.SetProgressIndicator(false);
                    contentContainer.IsEnabled = true;
                    foreach (var content in App.SubjectReviewViewModel.ReveiwContentList)
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
            });

        }

        private void appbarMenuHome_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }

    }
}