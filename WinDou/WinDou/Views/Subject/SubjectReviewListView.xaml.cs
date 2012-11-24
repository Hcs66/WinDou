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
using WinDou.ViewModels;

namespace WinDou.Views.Subject
{
    public partial class SubjectReviewListView : WinDouAppPage
    {
        private string m_SubjectId;
        private string m_SubjectType;
        public SubjectReviewListView()
        {
            InitializeComponent();
            this.DataContext = App.SubjectReviewListViewModel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New && NavigationContext.QueryString.ContainsKey("subjectId"))
            {
                m_SubjectId = NavigationContext.QueryString["subjectId"];
                m_SubjectType = NavigationContext.QueryString["subjectType"];
                App.SubjectReviewListViewModel.GetReviewsCompleted += new EventHandler<ViewModels.DoubanSearchCompletedEventArgs>(GetReviewsCompleted);
                base.SetProgressIndicator(true);
                App.SubjectReviewListViewModel.GetReviews(m_SubjectId, m_SubjectType);
            }
        }

        void GetReviewsCompleted(object s, DoubanSearchCompletedEventArgs args)
        {
            App.SubjectReviewListViewModel.GetReviewsCompleted -= GetReviewsCompleted;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (args.IsSuccess)
                {
                    listReview.Visibility = Visibility.Visible;
                }
                else
                {
                    ToastPrompt toast = new ToastPrompt();
                    toast.Message = "没有相关评论";
                    toast.Show();
                    NavigationService.GoBack();
                }
                base.SetProgressIndicator(false);
            });
        }

        private void btnLoadMore_Click(object sender, RoutedEventArgs e)
        {
            if (!App.SubjectReviewListViewModel.IsBusy)
            {
                App.SubjectReviewListViewModel.GetReviewsCompleted += new EventHandler<ViewModels.DoubanSearchCompletedEventArgs>(GetReviewsCompleted);
                base.SetProgressIndicator(true);
                App.SubjectReviewListViewModel.GetReviews(m_SubjectId, m_SubjectType, true);
            }
        }

        private void linkBtnReview_Click(object sender, RoutedEventArgs e)
        {
            string id = (sender as HyperlinkButton).CommandParameter.ToString();
            id = id.Substring(id.LastIndexOf("/") + 1);
            NavigationService.Navigate(new Uri("/Views/Subject/SubjectReviewView.xaml?reviewId=" + id + "", UriKind.Relative));
        }

        private void appbarMenuHome_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}