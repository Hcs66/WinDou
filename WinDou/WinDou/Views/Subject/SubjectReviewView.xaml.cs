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
using Coding4Fun.Phone.Controls;
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
            if (NavigationContext.QueryString.ContainsKey("reviewId"))
            {
                string reviewId = NavigationContext.QueryString["reviewId"];
                if (!string.IsNullOrEmpty(reviewId))
                {
                    App.SubjectReviewViewModel.GetReviewCompleted += new EventHandler<ViewModels.DoubanSearchCompletedEventArgs>(GetReviewCompleted);
                    App.SubjectReviewViewModel.GetReview(reviewId);
                }
            }

            base.OnNavigatedTo(e);
        }

        void GetReviewCompleted(object s, ViewModels.DoubanSearchCompletedEventArgs args)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (args.IsSuccess)
                {
                    contentContainer.Visibility = Visibility.Visible;
                    contentContainer.IsEnabled = false;
                    DataContext = args.Result;
                    this.SetProgressIndicator(true);
                    App.SubjectReviewViewModel.GetReviewContentCompleted += GetReviewContent_Completed;
                    App.SubjectReviewViewModel.FetchReviewContent();
                }
                else
                {
                    ToastPrompt toast = new ToastPrompt();
                    toast.Message = args.Message;
                    toast.Show();
                }
                //base.SetProgressIndicator(false);
            });
            App.SubjectReviewViewModel.GetReviewCompleted -= GetReviewCompleted;
        }

        private void appbarBack_Click(object sender, EventArgs e)
        {
            PagingContent("back");
        }

        private void appbarNext_Click(object sender, EventArgs e)
        {
            PagingContent("next");
        }

        private void PagingContent(string type)
        {
            App.SubjectReviewViewModel.CurrentPageIndex = type == "back" ? App.SubjectReviewViewModel.CurrentPageIndex - 1 : App.SubjectReviewViewModel.CurrentPageIndex + 1;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = App.SubjectReviewViewModel.CurrentPageIndex > 0;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = App.SubjectReviewViewModel.CurrentPageIndex + 1 < App.SubjectReviewViewModel.TotalPages;
            txtReviewContent.Text = App.SubjectReviewViewModel.ReveiwContentList[App.SubjectReviewViewModel.CurrentPageIndex];
            ApplicationTitle.Text = "WinDou-评论 " + (App.SubjectReviewViewModel.CurrentPageIndex + 1).ToString() + "/" + App.SubjectReviewViewModel.TotalPages.ToString();
            contentContainer.ScrollToVerticalOffset(0);
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            this.SetProgressIndicator(true);
            contentContainer.IsEnabled = false;
            App.SubjectReviewViewModel.GetReviewContentCompleted += GetReviewContent_Completed;
            App.SubjectReviewViewModel.FetchReviewContent();
        }

        void GetReviewContent_Completed(object s, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.SetProgressIndicator(false);
                    contentContainer.IsEnabled = true;
                    //btnView.Visibility = Visibility.Collapsed;
                    App.SubjectReviewViewModel.GetReviewContentCompleted -= GetReviewContent_Completed;
                    txtReviewContent.Text = App.SubjectReviewViewModel.ReveiwContentList[0];
                    if (App.SubjectReviewViewModel.TotalPages > 1)
                    {
                        ApplicationBar.IsVisible = true;
                        ApplicationTitle.Text = "WinDou-评论 " + (App.SubjectReviewViewModel.CurrentPageIndex + 1).ToString() + "/" + App.SubjectReviewViewModel.TotalPages.ToString();
                        ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
                        ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = App.SubjectReviewViewModel.TotalPages > 1;
                    }
                        contentContainer.ScrollToVerticalOffset(0);
                });
        }

        private void appbarMenuHome_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }

    }
}