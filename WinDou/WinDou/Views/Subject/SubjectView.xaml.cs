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

namespace WinDou.Views
{
    public partial class SubjectView : WinDouAppPage
    {
        private string m_SubjectId;
        private string m_SubjectType;
        public SubjectView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("subjectId"))
            {
                m_SubjectType = NavigationContext.QueryString["type"];
                SetContentTemplate();
                App.SubjectViewModel.GetSubjectCompleted += new EventHandler<ViewModels.DoubanSearchCompletedEventArgs>(SubjectViewModel_GetSubjectCompleted);
                m_SubjectId = NavigationContext.QueryString["subjectId"];
                App.SubjectViewModel.GetSubject(m_SubjectId, m_SubjectType);
            }

            base.OnNavigatedTo(e);
        }

        void SubjectViewModel_GetSubjectCompleted(object sender, ViewModels.DoubanSearchCompletedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                this.SubjectContent.Content = e.Result;
                this.SubjectContent.Visibility = Visibility.Visible;
                base.SetProgressIndicator(false);
            });
            App.SubjectViewModel.GetSubjectCompleted -= SubjectViewModel_GetSubjectCompleted;
        }

        private void SetContentTemplate()
        {
            switch (m_SubjectType)
            {
                case "0":
                    this.SubjectContent.ContentTemplate = this.Resources["BookTemplate"] as DataTemplate;
                    break;
                case "1":
                    this.SubjectContent.ContentTemplate = this.Resources["MovieTemplate"] as DataTemplate;
                    break;
                case "2":
                    this.SubjectContent.ContentTemplate = this.Resources["MusicTemplate"] as DataTemplate;
                    break;
                default:
                    break;
            }
        }

        private void appbarViewReview_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Subject/SubjectReviewListView.xaml?subjectId=" + m_SubjectId + "&subjectType=" + m_SubjectType, UriKind.Relative));
        }

        private void appbarMenuHome_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}