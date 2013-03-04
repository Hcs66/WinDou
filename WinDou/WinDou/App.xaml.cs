using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;
using Coding4Fun.Toolkit.Controls;
using DoubanSharp.Model;
using DoubanSharp.Service;
using HcsLib.WindowsPhone.Configuration;
using HcsLib.WindowsPhone.Msic;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Practices.Mobile.Configuration;
using WinDou.Resources;
using WinDou.ViewModels;

namespace WinDou
{
    public partial class App : Application
    {
        #region 字段

        private static MainViewModel mainViewModel = null;
        private static SearchSubjectViewModel searchSubjectViewModel = null;
        private static SubjectViewModel subjectViewModel = null;
        private static SubjectReviewViewModel subjectReviewViewModel = null;
        private static SubjectReviewListViewModel subjectReviewListViewModel = null;
        private static NewOfBooksViewModel newOfBooksViewModel = null;
        private static NewOfMoviesViewModel newOfMoviesViewModel = null;
        private static NewOfMusicViewModel newOfMusicViewModel = null;
        private static GroupViewModel groupViewModel = null;
        private static MyGroupViewModel myGroupViewModel = null;
        private static GroupTopicViewModel groupTopicViewModel = null;

        private static bool appInitialLoadPerformed = false;
        public static bool AppInitialized = false;
        private static DoubanService doubanService = null;
        private static ApplicationSettingsSection applicationSettings = null;

        #endregion

        #region 属性

        //应用程序配置
        public static ApplicationSettingsSection ApplicationSettings
        {
            get
            {
                if (applicationSettings == null)
                {
                    applicationSettings = (ApplicationSettingsSection)ConfigurationManager.GetSection("ApplicationSettings");
                }
                return applicationSettings;
            }
        }

        /// <summary>
        /// 主界面ViewModel
        /// </summary>
        /// <returns>The MainViewModel object.</returns>
        public static MainViewModel MainViewModel
        {
            get
            {
                if (mainViewModel == null)
                    mainViewModel = new MainViewModel();

                return mainViewModel;
            }
        }

        /// <summary>
        /// 搜索项目ViewModel
        /// </summary>
        public static SearchSubjectViewModel SearchSubjectViewModel
        {
            get
            {
                if (searchSubjectViewModel == null)
                    searchSubjectViewModel = new SearchSubjectViewModel();
                return searchSubjectViewModel;
            }
        }

        /// <summary>
        /// 项目ViewModel
        /// </summary>
        public static SubjectViewModel SubjectViewModel
        {
            get
            {
                if (subjectViewModel == null)
                    subjectViewModel = new SubjectViewModel();
                return subjectViewModel;
            }
        }

        /// <summary>
        /// 评论ViewModel
        /// </summary>
        public static SubjectReviewViewModel SubjectReviewViewModel
        {
            get
            {
                if (subjectReviewViewModel == null)
                    subjectReviewViewModel = new SubjectReviewViewModel();
                return subjectReviewViewModel;
            }
        }

        /// <summary>
        /// 评论列表ViewModel
        /// </summary>
        public static SubjectReviewListViewModel SubjectReviewListViewModel
        {
            get
            {
                if (subjectReviewListViewModel == null)
                    subjectReviewListViewModel = new SubjectReviewListViewModel();
                return subjectReviewListViewModel;
            }
        }

        /// <summary>
        /// 最新书籍ViewModel
        /// </summary>
        public static NewOfBooksViewModel NewOfBooksViewModel
        {
            get
            {
                if (newOfBooksViewModel == null)
                    newOfBooksViewModel = new NewOfBooksViewModel();
                return newOfBooksViewModel;
            }
        }

        /// <summary>
        /// 最新电影ViewModel
        /// </summary>
        public static NewOfMoviesViewModel NewOfMoviesViewModel
        {
            get
            {
                if (newOfMoviesViewModel == null)
                    newOfMoviesViewModel = new NewOfMoviesViewModel();
                return newOfMoviesViewModel;
            }
        }

        /// <summary>
        /// 最新音乐ViewModel
        /// </summary>
        public static NewOfMusicViewModel NewOfMusicViewModel
        {
            get
            {
                if (newOfMusicViewModel == null)
                    newOfMusicViewModel = new NewOfMusicViewModel();
                return newOfMusicViewModel;
            }
        }

        /// <summary>
        /// 小组ViewModel
        /// </summary>
        public static GroupViewModel GroupViewModel
        {
            get
            {
                if (groupViewModel == null)
                {
                    groupViewModel = new GroupViewModel();
                }
                return groupViewModel;
            }
        }

        /// <summary>
        /// 我的小组ViewModel
        /// </summary>
        public static MyGroupViewModel MyGroupViewModel
        {
            get
            {
                if (myGroupViewModel == null)
                {
                    myGroupViewModel = new MyGroupViewModel();
                }
                return myGroupViewModel;
            }
        }

        /// <summary>
        /// 小组话题ViewModel
        /// </summary>
        public static GroupTopicViewModel GroupTopicViewModel
        {
            get
            {
                if (groupTopicViewModel == null)
                {
                    groupTopicViewModel = new GroupTopicViewModel();
                }
                return groupTopicViewModel;
            }
        }

        /// <summary>
        /// 豆瓣服务类实体
        /// </summary>
        public static DoubanService DoubanService
        {
            get
            {
                if (doubanService == null)
                {
                    string cosumerKey = ApplicationSettings.AppSettings["ConsumerKey"].Value;
                    string consumerSecret = ApplicationSettings.AppSettings["ConsumerSecret"].Value;
                    string redirectUrl = ApplicationSettings.AppSettings["RedirectUrl"].Value;
                    doubanService = new DoubanService(cosumerKey, consumerSecret, redirectUrl);
                }
                return doubanService;
            }
        }

        /// <summary>
        /// Synchronization object used for safely updating the application data.
        /// </summary>
        public static object SyncObject { get; private set; }

        /// <summary>
        /// 应用程序数据是否化完毕
        /// </summary>
        public static bool IsAppDataLoaded
        {
            get
            {
                lock (SyncObject)
                {
                    return appInitialLoadPerformed;
                }
            }
        }

        /// <summary>
        /// Gets whether or not the application was restored from tombstoning.
        /// </summary>
        public static bool WasTombstoned { get; private set; }

        /// <summary>
        ///提供对电话应用程序的根框架的轻松访问。
        /// </summary>
        /// <returns>电话应用程序的根框架。</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        #endregion

        /// <summary>
        /// Application 对象的构造函数。
        /// </summary>
        public App()
        {
            // 未捕获的异常的全局处理程序。
            UnhandledException += Application_UnhandledException;

            // 标准 XAML 初始化
            InitializeComponent();

            // 特定于电话的初始化
            InitializePhoneApplication();

            // 语言显示初始化
            InitializeLanguage();

            // 调试时显示图形分析信息。
            if (Debugger.IsAttached)
            {
                // 显示当前帧速率计数器。
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // 显示在每个帧中重绘的应用程序区域。
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // 启用非生产分析可视化模式，
                // 该模式显示递交给 GPU 的包含彩色重叠区的页面区域。
                //Application.Current.Host.Settings.EnableCacheVisualization = true；

                // 通过禁用以下对象阻止在调试过程中关闭屏幕
                // 应用程序的空闲检测。
                //  注意: 仅在调试模式下使用此设置。禁用用户空闲检测的应用程序在用户不使用电话时将继续运行
                // 并且消耗电池电量。
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
            SyncObject = new object();
        }

        // 应用程序启动(例如，从“开始”菜单启动)时执行的代码
        // 此代码在重新激活应用程序时不执行
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            WasTombstoned = false;
            appInitialLoadPerformed = false;
            AppInitialized = false;
            //检测是否已授权
            OAuthAccessToken accessToken = IsolatedStorageHelper.LoadFile<OAuthAccessToken>(Globals.ACCESSTOKEN_FILENAME);//new OAuthAccessToken() { Token = "a1fb1863daf46b17e5f1492b90dd4145", TokenSecret = "76e18a2ba44fff88", UserId = "1351402" };
            if (accessToken != null && !string.IsNullOrEmpty(accessToken.AccessToken))
            {
                DoubanService.AuthenticateWith(accessToken);
            }
        }

        // 激活应用程序(置于前台)时执行的代码
        // 此代码在首次启动应用程序时不执行
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            if (!e.IsApplicationInstancePreserved)
            {
                WasTombstoned = true;
                App.AppInitialized = true;
            }
            else
            {
                WasTombstoned = false;
            }
        }

        // 停用应用程序(发送到后台)时执行的代码
        // 此代码在应用程序关闭时不执行
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // 应用程序关闭(例如，用户点击“后退”)时执行的代码
        // 此代码在停用应用程序时不执行
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // 导航失败时执行的代码
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // 导航已失败；强行进入调试器
                Debugger.Break();
            }
        }

        // 出现未处理的异常时执行的代码
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // 出现未处理的异常；强行进入调试器
                Debugger.Break();
            }
            //弹出错误信息
            MessagePrompt errorPrompt = new MessagePrompt();
            errorPrompt.ActionPopUpButtons.Clear();
            Button okButton = new Button();
            okButton.Content = "返回";
            okButton.Click+=(s,args)=>
                {
                    errorPrompt.OnCompleted(new PopUpEventArgs<string, PopUpResult>() { PopUpResult = PopUpResult.Cancelled });
                };
            errorPrompt.ActionPopUpButtons.Add(okButton);
            Button sendButton = new Button();
            sendButton.Content = "发送错误信息";
            sendButton.Click += (s, args) =>
            {
                EmailComposeTask emailTask = new EmailComposeTask();
                string errorMsg = "";
                if (e.ExceptionObject != null)
                {
                    errorMsg = "[Message]:" + e.ExceptionObject.Message + "\n\n\n[Source]:" + e.ExceptionObject.Source +
                        "\n\n\n[StackTrace]:" + e.ExceptionObject.StackTrace;            
                }
                emailTask.Body = errorMsg;
                emailTask.Subject = "WinDou应用错误报告";
                emailTask.To = "690090@qq.com";
                emailTask.Show();
            };
            errorPrompt.ActionPopUpButtons.Add(sendButton);
            errorPrompt.Message = "对不起，程序出错了，请重新尝试或重启应用";
            errorPrompt.Show();

            e.Handled = true;
        }

        #region 电话应用程序初始化

        // 避免双重初始化
        private bool phoneApplicationInitialized = false;

        // 请勿向此方法中添加任何其他代码
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // 创建框架但先不将它设置为 RootVisual；这允许初始
            // 屏幕保持活动状态，直到准备呈现应用程序时。
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // 处理导航故障
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // 在下一次导航中处理清除 BackStack 的重置请求，
            RootFrame.Navigated += CheckForResetNavigation;

            // 确保我们未再次初始化
            phoneApplicationInitialized = true;
        }

        // 请勿向此方法中添加任何其他代码
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // 设置根视觉效果以允许应用程序呈现
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // 删除此处理程序，因为不再需要它
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // 如果应用程序收到“重置”导航，则需要进行检查
            // 以确定是否应重置页面堆栈
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // 取消注册事件，以便不再调用该事件
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // 只为“新建”(向前)和“刷新”导航清除堆栈
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // 为了获得 UI 一致性，请清除整个页面堆栈
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // 不执行任何操作
            }
        }

        #endregion

        // 初始化应用程序在其本地化资源字符串中定义的字体和排列方向。
        //
        // 若要确保应用程序的字体与受支持的语言相符，并确保
        // 这些语言的 FlowDirection 都采用其传统方向，ResourceLanguage
        // 应该初始化每个 resx 文件中的 ResourceFlowDirection，以便将这些值与以下对象匹配
        // 文件的区域性。例如:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage 的值应为“es-ES”
        //    ResourceFlowDirection 的值应为“LeftToRight”
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage 的值应为“ar-SA”
        //     ResourceFlowDirection 的值应为“RightToLeft”
        //
        // 有关本地化 Windows Phone 应用程序的详细信息，请参见 http://go.microsoft.com/fwlink/?LinkId=262072。
        //
        private void InitializeLanguage()
        {
            try
            {
                // 将字体设置为与由以下对象定义的显示语言匹配
                // 每种受支持的语言的 ResourceLanguage 资源字符串。
                //
                // 如果显示出现以下情况，则回退到非特定语言的字体
                // 手机的语言不受支持。
                //
                // 如果命中编译器错误，则表示以下对象中缺少 ResourceLanguage
                // 资源文件。
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // 根据以下条件设置根框架下的所有元素的 FlowDirection
                // 每个以下对象的 ResourceFlowDirection 资源字符串上的
                // 受支持的语言。
                //
                // 如果命中编译器错误，则表示以下对象中缺少 ResourceFlowDirection
                // 资源文件。
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // 如果此处导致了异常，则最可能的原因是
                // ResourceLangauge 未正确设置为受支持的语言
                // 代码或 ResourceFlowDirection 设置为 LeftToRight 以外的值
                // 或 RightToLeft。

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }
    }
}