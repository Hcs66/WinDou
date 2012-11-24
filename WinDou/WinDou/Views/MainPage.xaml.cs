using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DoubanSharp.Model;
using Microsoft.Phone.Shell;
using System.Windows.Navigation;
using WinDou.Views;
using System.Windows.Controls.Primitives;
using Coding4Fun.Phone.Controls;
using System.ComponentModel;
using System.Threading;
using HcsLib.WindowsPhone.Msic;
using WinDou.ViewModels;
using System.Net.NetworkInformation;

namespace WinDou
{
    public partial class MainPage : WinDouAppPage
    {
        protected OAuthRequestToken requestToken;

        public MainPage()
        {
            InitializeComponent();

            InitializeAppBar();

            DataContext = App.MainViewModel;

            webBrowser.Navigated += webBrowser_Navigated;

            BackKeyPress += OnBackKeyPressed;
        }

        #region 回调事件

        private void OnBackKeyPressed(object sender, CancelEventArgs e)
        {
            var result = MessageBox.Show("确认退出吗？", "信息",
                                            MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                return;
            }
            e.Cancel = true;
        }

        void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            webBrowser.Visibility = Visibility.Visible;
            webBrowser.Navigated -= webBrowser_Navigated;
        }

        void MainViewModel_LoadCurrentPeopleCompleted(object sender, EventArgs e)
        {
            TogglePivotBusyStyle(true);
            App.MainViewModel.LoadCurrentPeopleCompleted -= MainViewModel_LoadCurrentPeopleCompleted;
        }

        void InitializeData_LoadSayingCompleted(object sender, EventArgs e)
        {
            TogglePivotBusyStyle(true);
            App.MainViewModel.LoadSayingCompleted -= InitializeData_LoadSayingCompleted;
        }

        protected override void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            base.OnPageLoaded(sender, e);
        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (App.DoubanService.HasAuthenticated)
            {
                switch (pivot.SelectedIndex)
                {
                    case 0:
                        ApplicationBar = this.Resources["appBarDefault"] as ApplicationBar;
                        break;
                    case 1:
                        ApplicationBar = this.Resources["appBarSaying"] as ApplicationBar;
                        break;
                    case 2:
                    case 3:
                    case 4:
                        ApplicationBar = this.Resources["appBarSubjectReview"] as ApplicationBar;
                        break;
                    default:
                        break;
                }
            }
        }

        void input_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult == PopUpResult.Ok)
            {
                InputPrompt input = sender as InputPrompt;
                App.MainViewModel.AddSayingCompleted += Saying_AddSayingCompleted;
                App.MainViewModel.AddSaying(input.Value);
            }
        }

        void Saying_AddSayingCompleted(object sender, DoubanSearchCompletedEventArgs e)
        {
            throw new Exception();
            App.MainViewModel.AddSayingCompleted -= Saying_AddSayingCompleted;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (!e.IsSuccess)
                {
                    ToastPrompt toast = new ToastPrompt();
                    toast.Message = e.Message;
                    toast.Show();
                }
                else
                {
                    appBarBtnRefreshSaying_Click(null, new EventArgs());
                }
            });
        }

        void Saying_LoadSayingCompleted(object sender, DoubanSearchCompletedEventArgs e)
        {
            App.MainViewModel.LoadSayingCompleted -= Saying_LoadSayingCompleted;
            if (!e.IsSuccess)
            {
                ToastPrompt toast = new ToastPrompt();
                toast.Message = e.Message;
                toast.Show();
            }
            ToggleListBoxBusyStyle(listSaying, true);
        }

        void MainViewModel_LoadMusicReviewCompleted(object sender, EventArgs e)
        {
            App.MainViewModel.LoadMusicReviewCompleted -= MainViewModel_LoadMusicReviewCompleted;
            ToggleListBoxBusyStyle(listMusicReview, true);
        }

        void MainViewModel_LoadMovieReviewCompleted(object sender, EventArgs e)
        {
            App.MainViewModel.LoadMovieReviewCompleted -= MainViewModel_LoadMovieReviewCompleted;
            ToggleListBoxBusyStyle(listMovieReview, true);
        }

        void MainViewModel_LoadBookReviewCompleted(object sender, EventArgs e)
        {
            App.MainViewModel.LoadBookReviewCompleted -= MainViewModel_LoadBookReviewCompleted;
            ToggleListBoxBusyStyle(listBookReview, true);
        }

        private void HandleGetRequestToken(OAuthRequestToken token, DoubanResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                requestToken = token;
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    webBrowser.Navigate(App.DoubanService.GetAuthorizationUri(token));
                });
            }
        }

        #endregion

        #region AppBar

        private void InitializeAppBar()
        {
            if (App.DoubanService.HasAuthenticated)
            {
                ApplicationBar = this.Resources["appBarDefault"] as ApplicationBar;
            }
            else
            {
                ApplicationBar = this.Resources["appBarAuthenticate"] as ApplicationBar;
            }
        }

        private void appBarBtnConfirm_Click(object sender, EventArgs e)
        {
            ApplicationBar = this.Resources["appBarDefault"] as ApplicationBar;
            App.DoubanService.GetAccessToken(requestToken, (accessToken, accessResp) =>
            {
                if (accessResp.StatusCode == HttpStatusCode.OK)
                {
                    App.DoubanService.AuthenticateWith(accessToken.Token, accessToken.TokenSecret, accessToken.UserId);
                    //保存token
                    IsolatedStorageHelper.SaveFile<OAuthAccessToken>(Globals.ACCESSTOKEN_FILENAME, accessToken);
                    //验证后加载数据
                    Deployment.Current.Dispatcher.BeginInvoke(()
                        =>
                    {
                        webBrowser.Visibility = Visibility.Collapsed;
                        InitializeData();
                    }
                    );
                }
                else
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("授权失败，请重新授权", "提示", MessageBoxButton.OK);
                    });
                }
            });
        }

        private void appBarBtnRetryAuthenticate_Click(object sender, EventArgs e)
        {
            App.DoubanService.GetRequestToken(HandleGetRequestToken);
        }

        private void appBarBtnAddSying_Click(object sender, EventArgs e)
        {
            InputPrompt input = new InputPrompt();
            input.IsCancelVisible = true;
            input.Message = "";
            input.Title = "发表说说";
            input.Completed += new EventHandler<PopUpEventArgs<string, PopUpResult>>(input_Completed);
            input.Show();
        }

        private void appBarBtnRefreshSaying_Click(object sender, EventArgs e)
        {
            if (App.MainViewModel.SayingList.Count > 0)
            {
                listSaying.ScrollTo(App.MainViewModel.SayingList[0]);
            }
            ToggleListBoxBusyStyle(listSaying, false);
            App.MainViewModel.LoadSayingCompleted += Saying_LoadSayingCompleted;
            App.MainViewModel.RefreshSayingData();
        }

        private void appBarBtnSearchSubject_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Subject/SearchSubject.xaml?type=" + (pivot.SelectedIndex - 2).ToString(), UriKind.Relative));
        }

        private void appBarBtnRefreshSubject_Click(object sender, EventArgs e)
        {
            switch (pivot.SelectedIndex)
            {
                case 2:
                    ToggleListBoxBusyStyle(listBookReview, false);
                    App.MainViewModel.LoadBookReviewCompleted += new EventHandler(MainViewModel_LoadBookReviewCompleted);
                    App.MainViewModel.LoadBookReviewData(true);
                    break;
                case 3:
                    ToggleListBoxBusyStyle(listMovieReview, false);
                    App.MainViewModel.LoadMovieReviewCompleted += new EventHandler(MainViewModel_LoadMovieReviewCompleted);
                    App.MainViewModel.LoadMovieReviewData(true);
                    break;
                case 4:
                    ToggleListBoxBusyStyle(listMusicReview, false);
                    App.MainViewModel.LoadMusicReviewCompleted += new EventHandler(MainViewModel_LoadMusicReviewCompleted);
                    App.MainViewModel.LoadMusicReviewData(true);
                    break;
                default:
                    break;
            }
        }

        private void appBarBtnRetryLogin_Click(object sender, EventArgs e)
        {
            //重置以保存的token
            App.DoubanService.ResetAuthenticate();
            IsolatedStorageHelper.SaveFile<OAuthAccessToken>(Globals.ACCESSTOKEN_FILENAME, new OAuthAccessToken());
            pivot.Visibility = Visibility.Collapsed;
            ApplicationBar = this.Resources["appBarAuthenticate"] as ApplicationBar;
            ApplicationBar.IsVisible = true;
            base.SetProgressIndicator(false);
            webBrowser.Navigated += webBrowser_Navigated;
            App.DoubanService.GetRequestToken(HandleGetRequestToken);
        }

        #endregion

        #region Tile

        private void tileSearchSubject_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Subject/SearchSubject.xaml?type=0", UriKind.Relative));

        }

        private void tileNewBooks_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/NewOfBooksView.xaml", UriKind.Relative));
        }

        private void tileNewMovies_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/NewOfMoviesView.xaml", UriKind.Relative));
        }

        private void tileNewMusic_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/NewOfMusicView.xaml", UriKind.Relative));
        }

        private void tileSetting_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tileAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutPrompt about = new AboutPrompt();
            about.Show("Hcs66", "", "690090@qq.com", "www.geekandlife.info");
        }

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            base.SetProgressIndicator(false);
            if (NetworkInterface.GetIsNetworkAvailable())//无网络时提示
            {
                if(MessageBox.Show("请确保可以访问网络", "提示信息", MessageBoxButton.OK)== MessageBoxResult.OK)
                {
                    
                };
            }
            else
            {
                //未授权进入授权处理
                if (!App.DoubanService.HasAuthenticated)
                {
                    //TogglePivotBusyStyle(false);
                    pivot.Visibility = Visibility.Collapsed;
                    base.SetProgressIndicator(false);
                    ApplicationBar.IsVisible = true;
                    App.DoubanService.GetRequestToken(HandleGetRequestToken);
                }
                else//已授权,数据初始化
                {
                    lock (App.SyncObject)
                    {
                        if (!App.MainViewModel.IsDataLoaded)
                        {
                            InitializeData();
                        }
                    }
                }
            }
            base.OnNavigatedTo(e);
        }

        void InitializeData()
        {
            App.MainViewModel.LoadCurrentPeopleCompleted += new EventHandler(MainViewModel_LoadCurrentPeopleCompleted);
            App.MainViewModel.LoadSayingCompleted += Saying_LoadSayingCompleted;
            TogglePivotBusyStyle(false);
            App.MainViewModel.LoadData();
        }

        private void ToggleListBoxBusyStyle(Control list, bool isEnabled)
        {
            list.IsEnabled = isEnabled;
            base.SetProgressIndicator(!isEnabled);
        }

        private void TogglePivotBusyStyle(bool isVisible)
        {
            pivot.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            ApplicationBar.IsVisible = isVisible;
            base.SetProgressIndicator(!isVisible);
        }

        private void linkBtnReview_Click(object sender, RoutedEventArgs e)
        {
            string id = (sender as RoundButton).CommandParameter.ToString();
            NavigationService.Navigate(new Uri("/Views/Subject/SubjectReviewView.xaml?reviewId=" + id + "", UriKind.Relative));
        }

        private void btnLoadMoreSaying_Click(object sender, RoutedEventArgs e)
        {
            ToggleListBoxBusyStyle(listSaying, false);
            App.MainViewModel.LoadSayingCompleted += Saying_LoadSayingCompleted;
            App.MainViewModel.LoadMoreSayingData();
        }

        private void linkBtnViewSubject_Click(object sender, RoutedEventArgs e)
        {
            RoundButton button = sender as RoundButton;
            if (button.CommandParameter != null && !string.IsNullOrEmpty(button.CommandParameter.ToString()))
            {
                string url = "/Views/Subject/SubjectView.xaml?subjectId=" + button.CommandParameter.ToString() + "&type=" + (pivot.SelectedIndex - 2).ToString();
                NavigationService.Navigate(new Uri(url, UriKind.Relative));
            }
        }



    }
}