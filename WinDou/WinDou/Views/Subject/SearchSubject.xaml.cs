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

namespace WinDou.Views
{
    public partial class SearchSubject : WinDouAppPage
    {
        public SearchSubject()
        {
            InitializeComponent();
            DataContext = App.SearchSubjectViewModel;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtKeyWord.Text))
            {
                ToastPrompt prompt = new ToastPrompt();
                prompt.Message = "请输入关键字";
                prompt.Show();
                return;
            }
            if (!App.SearchSubjectViewModel.IsBusy)
            {
                SetListItemTemplate();//设置模板
                btnSearch.IsEnabled = false;
                base.SetProgressIndicator(true);
                App.SearchSubjectViewModel.SearchCompleted += new EventHandler<DoubanSearchCompletedEventArgs>(SearchSubjectViewModel_SearchCompleted);
                App.SearchSubjectViewModel.SearchSubject();
            }
        }

        private void SetListItemTemplate()
        {
            int type = lpSearchType.SelectedIndex;
            switch (type)
            {
                case 0:
                    listSearchResult.ItemTemplate = this.Resources["BookListTemplate"] as DataTemplate;
                    break;
                case 1:
                    listSearchResult.ItemTemplate = this.Resources["MovieListTemplate"] as DataTemplate;
                    break;
                case 2:
                    listSearchResult.ItemTemplate = this.Resources["MusicListTemplate"] as DataTemplate;
                    break;
                default:
                    break;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("type"))
            {
                int searchType = int.Parse(NavigationContext.QueryString["type"]);
                App.SearchSubjectViewModel.SearchType = lpSearchType.SelectedIndex = searchType;
            }
            base.SetProgressIndicator(false);
        }

        protected override void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            base.OnPageLoaded(sender, e);
            txtKeyWord.Focus();
        }

        void SearchSubjectViewModel_SearchCompleted(object sender, DoubanSearchCompletedEventArgs e)
        {
            App.SearchSubjectViewModel.SearchCompleted -= SearchSubjectViewModel_SearchCompleted;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    btnSearch.IsEnabled=true;
                });
            if (e.IsSuccess)
            {
                listSearchResult.UpdateLayout();
                listSearchResult.Visibility = Visibility.Visible;
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    ToastPrompt toast = new ToastPrompt();
                    toast.Message = "没有搜索到任何信息";
                    toast.Show();
                });
            }
            base.SetProgressIndicator(false);

        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            string subjectId = ((RoundButton)sender).CommandParameter.ToString();
            NavigationService.Navigate(new Uri("/Views/Subject/SubjectView.xaml?subjectId=" + subjectId + "&type=" + App.SearchSubjectViewModel.SearchType.ToString(), UriKind.Relative));
        }

        private void btnLoadMore_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtKeyWord.Text))
            {
                ToastPrompt prompt = new ToastPrompt();
                prompt.Message = "请输入关键字";
                prompt.Show();
                return;
            }
            if (!App.SearchSubjectViewModel.IsBusy)
            {
                base.SetProgressIndicator(true);
                App.SearchSubjectViewModel.SearchCompleted += new EventHandler<DoubanSearchCompletedEventArgs>(SearchSubjectViewModel_SearchCompleted);
                App.SearchSubjectViewModel.SearchSubject(true);
            }
        }

        private void appbarMenuHome_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}