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
using Coding4Fun.Toolkit.Controls;
using System.ComponentModel;
using System.Threading;
using HcsLib.WindowsPhone.Msic;
using WinDou.ViewModels;
using System.Net.NetworkInformation;
using Microsoft.Phone.Controls;

namespace WinDou
{
    public partial class MainPage : WinDouAppPage
    {
        public MainPage()
        {
            InitializeComponent();

            InitializeAppBar();

            DataContext = App.MainViewModel;

            BackKeyPress += OnBackKeyPressed;
        }



        #region 回调事件

        private void OnBackKeyPressed(object sender, CancelEventArgs e)
        {
            var result = MessageBox.Show("确认退出吗？", "信息",
                                            MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                App.Current.Terminate();
            }
            e.Cancel = true;
        }

        void webBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            if (e.Uri.Host == "localhost")
            {
                webBrowser.Navigating -= webBrowser_Navigating;
                webBrowser.Visibility = System.Windows.Visibility.Collapsed;
                string query = e.Uri.PathAndQuery;
                string code = query.Substring(query.IndexOf("=") + 1);
                //获取accesstoken
                App.DoubanService.Authenticate(code,
                    (accessToken, resp) =>
                    {
                        if (resp.RestResponse.StatusCode == HttpStatusCode.OK)
                        {
                            App.DoubanService.AuthenticateWith(accessToken);
                            //保存token
                            IsolatedStorageHelper.SaveFile<OAuthAccessToken>(Globals.ACCESSTOKEN_FILENAME, accessToken);
                            //验证后加载数据
                            Deployment.Current.Dispatcher.BeginInvoke(()
                                =>
                            {
                                pivot.Visibility = Visibility.Visible;
                                ApplicationBar = this.Resources["appBarDefault"] as ApplicationBar;
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
                    }
                    );
            }
        }

        void MainViewModel_LoadCurrentPeopleCompleted(object sender, EventArgs e)
        {
            App.MainViewModel.LoadCurrentPeopleCompleted -= MainViewModel_LoadCurrentPeopleCompleted;
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
                this.SetProgressIndicator(true);
                listSaying.IsBusy = true;
                InputPrompt input = sender as InputPrompt;
                App.MainViewModel.AddSayingCompleted += Saying_AddSayingCompleted;
                App.MainViewModel.AddSaying(input.Value);
            }
        }

        void Saying_AddSayingCompleted(object sender, DoubanSearchCompletedEventArgs e)
        {
            App.MainViewModel.AddSayingCompleted -= Saying_AddSayingCompleted;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                ToastPrompt toast = new ToastPrompt();
                this.SetProgressIndicator(false);
                listSaying.IsBusy = false;
                if (!e.IsSuccess)
                {
                    toast.Message = e.Message;
                    toast.Show();
                }
                else
                {
                    toast.Message = "发表成功！";
                    toast.Show();
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
            listSaying.UpdateLayout();
            ToggleListBoxBusyStyle(listSaying, false);
        }

        void MainViewModel_LoadMusicReviewCompleted(object sender, EventArgs e)
        {
            App.MainViewModel.LoadMusicReviewCompleted -= MainViewModel_LoadMusicReviewCompleted;
            ToggleListBoxBusyStyle(listMusicReview, false);
        }

        void MainViewModel_LoadMovieReviewCompleted(object sender, EventArgs e)
        {
            App.MainViewModel.LoadMovieReviewCompleted -= MainViewModel_LoadMovieReviewCompleted;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                ToggleListBoxBusyStyle(listMovieReview, false);
            });
        }

        void MainViewModel_LoadBookReviewCompleted(object sender, EventArgs e)
        {
            App.MainViewModel.LoadBookReviewCompleted -= MainViewModel_LoadBookReviewCompleted;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                ToggleListBoxBusyStyle(listBookReview, false);
            });
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

        private void appBarBtnRetryAuthenticate_Click(object sender, EventArgs e)
        {
            webBrowser.Navigate(App.DoubanService.GetAuthorizationUri());
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
            ToggleListBoxBusyStyle(listSaying, true);
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
                    ToggleListBoxBusyStyle(listBookReview, true);
                    App.MainViewModel.LoadBookReviewCompleted += new EventHandler(MainViewModel_LoadBookReviewCompleted);
                    App.MainViewModel.LoadBookReviewData(true);
                    break;
                case 3:
                    ToggleListBoxBusyStyle(listMovieReview, true);
                    App.MainViewModel.LoadMovieReviewCompleted += new EventHandler(MainViewModel_LoadMovieReviewCompleted);
                    App.MainViewModel.LoadMovieReviewData(true);
                    break;
                case 4:
                    ToggleListBoxBusyStyle(listMusicReview, true);
                    App.MainViewModel.LoadMusicReviewCompleted += new EventHandler(MainViewModel_LoadMusicReviewCompleted);
                    App.MainViewModel.LoadMusicReviewData(true);
                    break;
                default:
                    break;
            }
        }

        private void appBarBtnRetryLogin_Click(object sender, EventArgs e)
        {
            //年龄提示
            if (MessageBox.Show("根据微软市场规定，绑定已有帐号或者注册新账号需要确认您的年龄大于13岁", "温馨提示", MessageBoxButton.OKCancel)
                == MessageBoxResult.OK)
            {
                //重置保存的token
                App.DoubanService.ResetAuthenticate();
                //删除个人信息缓存
                IsolatedStorageHelper.DeleteFile(Globals.ACCESSTOKEN_FILENAME);
                IsolatedStorageHelper.DeleteFile(Globals.PEOPLE_FILENAME);
                IsolatedStorageHelper.DeleteFile(Globals.SAYINGLIST_FILENAME);
                App.MainViewModel.ClearUserData();
                ApplicationBar = this.Resources["appBarAuthenticate"] as ApplicationBar;
                HandleAuthenticate();
            }
        }

        private void appBarBtnAbout_Click(object sender, EventArgs e)
        {
            AboutPrompt about = new AboutPrompt();
            about.Background = new SolidColorBrush() { Color = Colors.Green };
            about.Show("Hcs66", "", "690090@qq.com", "www.5jiang5zhi.com");
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

        private void tileGroup_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Group/MyGroupView.xaml", UriKind.Relative));
        }

        private void tileAbout_Click(object sender, RoutedEventArgs e)
        {
            MessagePrompt msgPrompt = new MessagePrompt() { Title = "说明" };
            StackPanel aboutPanel = new StackPanel();
            TextBlock text1 = new TextBlock() { Text = "1.非常感谢使用本应用" };
            TextBlock text2 = new TextBlock() { Text = "2.由于豆瓣提供的API的限制，部分信息（如：评论详情）需要通过网页抓取", TextWrapping = TextWrapping.Wrap };
            TextBlock text3 = new TextBlock() { Text = "3.由于豆瓣没有对外提供小组相关API，目前使用该类API前需通过网页登录一次", TextWrapping = TextWrapping.Wrap };
            TextBlock text4 = new TextBlock() { Text = "4.有任何建议和意见欢迎随时反馈到690090@qq.com", TextWrapping = TextWrapping.Wrap };
            TextBlock text5 = new TextBlock() { Text = "PS:由于豆瓣提供的API目前还不是很完善，所以如果遇到信息无法获取请重新尝试，我也会继续优化", TextWrapping = TextWrapping.Wrap };
            aboutPanel.Children.Add(text1);
            aboutPanel.Children.Add(text2);
            aboutPanel.Children.Add(text3);
            aboutPanel.Children.Add(text4);
            aboutPanel.Children.Add(text5);
            msgPrompt.Body = aboutPanel;
            msgPrompt.Show();
        }

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!NetworkInterface.GetIsNetworkAvailable())//无网络时提示
            {
                if (MessageBox.Show("请确保可以访问网络", "提示信息", MessageBoxButton.OK) == MessageBoxResult.OK)
                {

                };
            }
            else
            {
                //未授权进入授权处理
                if (!App.DoubanService.HasAuthenticated)
                {
                    //年龄提示
                    if (MessageBox.Show("根据微软市场规定，绑定已有帐号或者注册新账号需要确认您的年龄大于13岁", "温馨提示", MessageBoxButton.OKCancel)
                        == MessageBoxResult.OK)
                    {
                        HandleAuthenticate();
                    }
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
        }

        private void HandleAuthenticate()
        {
            pivot.Visibility = Visibility.Collapsed;
            ApplicationBar.IsVisible = true;
            webBrowser.Visibility = System.Windows.Visibility.Visible;
            webBrowser.Navigating += webBrowser_Navigating;
            webBrowser.Navigate(App.DoubanService.GetAuthorizationUri());
        }

        void InitializeData()
        {
            App.MainViewModel.LoadCurrentPeopleCompleted += new EventHandler(MainViewModel_LoadCurrentPeopleCompleted);
            App.MainViewModel.LoadSayingCompleted += Saying_LoadSayingCompleted;
            App.MainViewModel.LoadBookReviewCompleted += new EventHandler(MainViewModel_LoadBookReviewCompleted);
            App.MainViewModel.LoadMovieReviewCompleted += new EventHandler(MainViewModel_LoadMovieReviewCompleted);
            App.MainViewModel.LoadMusicReviewCompleted += new EventHandler(MainViewModel_LoadMusicReviewCompleted);
            ToggleListBoxBusyStyle(listSaying, true);
            ToggleListBoxBusyStyle(listBookReview, true);
            ToggleListBoxBusyStyle(listMovieReview, true);
            ToggleListBoxBusyStyle(listMusicReview, true);
            App.MainViewModel.LoadData();
        }

        private void TogglePivotBusyStyle(bool isVisible)
        {
            pivot.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            ApplicationBar.IsVisible = isVisible;
        }

        private void linkBtnReview_Click(object sender, RoutedEventArgs e)
        {
            string id = (sender as RoundButton).CommandParameter.ToString();
            NavigationService.Navigate(new Uri("/Views/Subject/SubjectReviewView.xaml?reviewId=" + id + "", UriKind.Relative));
        }

        private void btnLoadMoreSaying_Click(object sender, RoutedEventArgs e)
        {
            ToggleListBoxBusyStyle(listSaying, true);
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