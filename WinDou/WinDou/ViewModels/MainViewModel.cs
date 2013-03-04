using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using DoubanSharp.Model;
using System.Xml;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
//using DoubanSharp.Model.Enum;
using HcsLib.WindowsPhone.ViewModel;
using HcsLib.WindowsPhone.Msic;
using System.Windows;


namespace WinDou.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private int m_RowPerPages = 20;
        private int m_CurrentSayingPageIndex = 0;
        private static Regex m_HtmlTagRegex = new Regex(@"</?[a-z][a-z0-9]*[^<>]*>");
        private static Regex m_ImgSrcRegex = new Regex("src=\"([^=]+)\"");
        private static Regex m_SubjectId = new Regex("/subject/([\\d]+)/");
        private RelayCommand m_AddSayingCommand;
        private static string m_BookReviewFeedUrl = "http://www.douban.com/feed/review/book_latest";
        private static string m_MovieReviewFeedUrl = "http://www.douban.com/feed/review/movie_latest";
        private static string m_MusicReviewFeedUrl = "http://www.douban.com/feed/review/music_latest";

        public MainViewModel()
        {
            this.SayingList = new ObservableCollection<DoubanMiniBlog>();
            this.BookReviewList = new ObservableCollection<DoubanSubjectReview>();
            this.MovieReviewList = new ObservableCollection<DoubanSubjectReview>();
            this.MusicReviewList = new ObservableCollection<DoubanSubjectReview>();
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(MainViewModel_PropertyChanged);
        }

        #region 属性

        public ObservableCollection<DoubanMiniBlog> SayingList { get; set; }

        public ObservableCollection<DoubanSubjectReview> BookReviewList { get; set; }

        public ObservableCollection<DoubanSubjectReview> MovieReviewList { get; set; }

        public ObservableCollection<DoubanSubjectReview> MusicReviewList { get; set; }

        public event EventHandler<DoubanSearchCompletedEventArgs> LoadSayingCompleted;

        public event EventHandler LoadBookReviewCompleted;

        public event EventHandler LoadMovieReviewCompleted;

        public event EventHandler LoadMusicReviewCompleted;

        public event EventHandler LoadCurrentPeopleCompleted;

        public event EventHandler<DoubanSearchCompletedEventArgs> AddSayingCompleted;

        public DoubanPeople CurrentPeople { get; set; }

        public Visibility SayingLoadMoreVisibility { get; set; }

        #endregion

        #region 方法

        void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
            switch (propertyName)
            {
                case "BookReviewList":
                    if (LoadBookReviewCompleted != null)
                    {
                        LoadBookReviewCompleted(this, null);
                    }
                    break;
                case "MovieReviewList":
                    if (LoadMovieReviewCompleted != null)
                    {
                        LoadMovieReviewCompleted(this, null);
                    }
                    break;
                case "MusicReviewList":
                    if (LoadMusicReviewCompleted != null)
                    {
                        LoadMusicReviewCompleted(this, null);
                    }
                    break;
                case "CurrentPeople":
                    if (LoadCurrentPeopleCompleted != null)
                    {
                        LoadCurrentPeopleCompleted(this, null);
                    }
                    break;
                case "SayingList":
                    if (LoadSayingCompleted != null)
                    {
                        LoadSayingCompleted(null, new DoubanSearchCompletedEventArgs() { IsSuccess = true });
                    }
                    break;
                default:
                    break;
            }
        }

        public void LoadMoreSayingData()
        {
            LoadSayingData(false, true);
        }

        public void RefreshSayingData()
        {
            LoadSayingData(true, false);
        }

        private void LoadSayingData(bool isRefresh = false, bool isPaging = false)
        {
            if (isRefresh)
            {
                m_CurrentSayingPageIndex = 0;
                this.SayingList = new ObservableCollection<DoubanMiniBlog>();
            }
            else if (isPaging)
            {
                m_CurrentSayingPageIndex += m_RowPerPages;
            }
            else
            {
                List<DoubanMiniBlog> cacheList = IsolatedStorageHelper.LoadFile<List<DoubanMiniBlog>>(Globals.SAYINGLIST_FILENAME);
                if (cacheList != null)
                {
                    this.SayingList = new ObservableCollection<DoubanMiniBlog>(cacheList);
                    this.OnPropertyChanged("SayingList");
                    return;
                }
            }
            App.DoubanService.SearchMiniBlogs(m_CurrentSayingPageIndex.ToString(), m_RowPerPages.ToString(),
                     (results, resp) =>
                     {
                         if (resp.RestResponse.StatusCode == HttpStatusCode.OK)
                         {
                             if (results.EntityList.Count <= 0)
                             {
                                 System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    if (isPaging)
                                    {
                                        m_CurrentSayingPageIndex = m_CurrentSayingPageIndex > m_RowPerPages ? m_CurrentSayingPageIndex - m_RowPerPages : 1;
                                        if (LoadSayingCompleted != null)
                                        {
                                            LoadSayingCompleted(null, new DoubanSearchCompletedEventArgs() { IsSuccess = false, Message = "没有更多的广播了" });
                                        }
                                    }
                                    else
                                    {
                                        SayingLoadMoreVisibility = Visibility.Collapsed;
                                        this.OnPropertyChanged("SayingLoadMoreVisibility");
                                        m_CurrentSayingPageIndex = 0;
                                        this.SayingList = new ObservableCollection<DoubanMiniBlog>();
                                        this.OnPropertyChanged("SayingList");

                                    }
                                });
                             }
                             else if (results.EntityList != null && results.EntityList.Count > 0)
                             {
                                 System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                                 {
                                     foreach (var item in results.EntityList)
                                     {
                                         item.Text = m_HtmlTagRegex.Replace(item.Text, "");
                                         if (string.IsNullOrEmpty(item.Text))
                                         {
                                             item.Text = item.Title;
                                             if (item.Attachments.Count > 0 && !string.IsNullOrEmpty(item.Attachments[0].Title))
                                             {
                                                 item.Text += ":" + item.Attachments[0].Title;
                                             }
                                         }
                                         item.CreatedAt = DateTime.Parse(item.CreatedAt).ToString("yyyy-MM-dd hh:mm:ss");
                                         this.SayingList.Add(item);
                                     }
                                     //to-do:返回的接口没有总数信息
                                     //int totalResults = int.Parse(results.Total);
                                     //SayingLoadMoreVisibility = SayingList.Count < totalResults ? Visibility.Visible : Visibility.Collapsed;
                                     //this.OnPropertyChanged("SayingLoadMoreVisibility");
                                     //缓存
                                     IsolatedStorageHelper.SaveFile<List<DoubanMiniBlog>>(Globals.SAYINGLIST_FILENAME, results.EntityList);
                                     this.OnPropertyChanged("SayingList");
                                 });
                             }
                         }
                         else//出错后序号置为原值
                         {
                             m_CurrentSayingPageIndex = m_CurrentSayingPageIndex > m_RowPerPages ? m_CurrentSayingPageIndex - m_RowPerPages : 1;
                             if (LoadSayingCompleted != null)
                             {
                                 LoadSayingCompleted(null, new DoubanSearchCompletedEventArgs() { IsSuccess = false, Message = "加载出错，请重试" });
                             }
                         }
                     }
                 );
        }

        private void LoadFeeds(Uri uri, bool isRefresh, string cacheFileName, Action<List<DoubanSubjectReview>> action)
        {
            if (!isRefresh)
            {
                List<DoubanSubjectReview> cacheList = IsolatedStorageHelper.LoadFile<List<DoubanSubjectReview>>(cacheFileName);
                if (cacheList != null)
                {
                    action(cacheList);
                    return;
                }
            }
            var feedsRequest = (HttpWebRequest)WebRequest.Create(uri);
            feedsRequest.BeginGetResponse(r =>
            {
                var httpRequest = (HttpWebRequest)r.AsyncState;
                var httpResponse = (HttpWebResponse)httpRequest.EndGetResponse(r);
                using (var reader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    XmlReader xmlReader = XmlReader.Create(reader);
                    SyndicationFeed feeds = SyndicationFeed.Load(xmlReader);
                    var feedItems = from feed in feeds.Items
                                    select new DoubanSubjectReview()
                                    {
                                        Id = feed.Id.Substring(feed.Id.TrimEnd('/').LastIndexOf("/") + 1).TrimEnd('/'),
                                        Title = feed.Title.Text,
                                        //Links = new List<DoubanLink>() { new DoubanLink() { Href = feed.Links[0].Uri.AbsoluteUri } },
                                        Summary = feed.Summary.Text,
                                        Updated = feed.PublishDate.LocalDateTime.ToShortDateString(),
                                        RawSource = feed.ElementExtensions.ReadElementExtensions<string>("encoded", "http://purl.org/rss/1.0/modules/content/")[0],
                                        Author = new DoubanAuthor() { Name = feed.ElementExtensions.ReadElementExtensions<string>("creator", "http://purl.org/dc/elements/1.1/")[0] }
                                    };
                    //匹配图片和项目Id
                    List<DoubanSubjectReview> reviewList = feedItems.ToList<DoubanSubjectReview>();
                    foreach (var review in reviewList)
                    {
                        if (m_ImgSrcRegex.IsMatch(review.RawSource))
                        {
                            review.Image = m_ImgSrcRegex.Match(review.RawSource).Groups[1].Value;
                        }
                        if (m_SubjectId.IsMatch(review.RawSource))
                        {
                            review.SubjectId = m_SubjectId.Match(review.RawSource).Groups[1].Value;
                        }
                    }
                    //缓存
                    IsolatedStorageHelper.SaveFile<List<DoubanSubjectReview>>(cacheFileName, reviewList);
                    action(reviewList);
                }

            }, feedsRequest);
        }

        public void LoadBookReviewData(bool isRefresh = false)
        {
            LoadFeeds(
                new Uri(m_BookReviewFeedUrl),
                isRefresh,
                Globals.BOOKREVIEWLIST_FILENAME,
                list =>
                {
                    this.BookReviewList = new ObservableCollection<DoubanSubjectReview>(list);
                    System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        this.OnPropertyChanged("BookReviewList");
                    });
                }
            );
        }

        public void LoadMovieReviewData(bool isRefresh = false)
        {
            LoadFeeds(
                new Uri(m_MovieReviewFeedUrl),
                isRefresh,
                Globals.MOVIEWREVIEWLIST_FILENAME,
                list =>
                {
                    this.MovieReviewList = new ObservableCollection<DoubanSubjectReview>(list);
                    System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        this.OnPropertyChanged("MovieReviewList");
                    });
                }
            );
        }

        public void LoadMusicReviewData(bool isRefresh = false)
        {
            LoadFeeds(
                new Uri(m_MusicReviewFeedUrl),
                isRefresh,
                Globals.MUSICREVIEWLIST_FILENAME,
                list =>
                {
                    this.MusicReviewList = new ObservableCollection<DoubanSubjectReview>(list);
                    System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        this.OnPropertyChanged("MusicReviewList");
                    });
                }
            );
        }

        public void LoadCurrentPeople()
        {
            DoubanPeople cachePeople = IsolatedStorageHelper.LoadFile<DoubanPeople>(Globals.PEOPLE_FILENAME);
            if (cachePeople != null)
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.CurrentPeople = cachePeople;
                    this.OnPropertyChanged("CurrentPeople");
                });
            }
            else
            {
                App.DoubanService.GetPeople(App.DoubanService.DoubanUserId,
                    (people, resp) =>
                    {
                        if (resp.RestResponse.StatusCode == HttpStatusCode.OK)
                        {
                            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                this.CurrentPeople = people;
                                IsolatedStorageHelper.SaveFile<DoubanPeople>(Globals.PEOPLE_FILENAME, people);
                                this.OnPropertyChanged("CurrentPeople");
                            });
                        }
                        else if (LoadCurrentPeopleCompleted != null)
                        {
                            LoadCurrentPeopleCompleted(this, null);
                        }
                    });
            }
        }


        public override void LoadData()
        {
            //加载当前用户信息
            LoadCurrentPeople();

            //加载说说列表
            LoadSayingData();

            //加载书评 
            LoadBookReviewData();

            //加载影评
            LoadMovieReviewData();

            //加载乐评
            LoadMusicReviewData();

            this.IsDataLoaded = true;
        }

        public void AddSaying(string message)
        {
            App.DoubanService.AddMiniBlog(new DoubanSharp.Model.DoubanMiniBlog() { Text = message }, message,
                        resp =>
                        {
                            if (AddSayingCompleted != null)
                            {
                                AddSayingCompleted(null, new DoubanSearchCompletedEventArgs() { IsSuccess = resp.RestResponse.StatusCode == HttpStatusCode.Created || resp.RestResponse.StatusCode == HttpStatusCode.OK, Message = resp.RestResponse.Content });
                            }
                        });
        }

        #endregion

        #region 命令

        public RelayCommand AddSayingCommand
        {
            get
            {
                if (this.m_AddSayingCommand == null)
                {
                    this.m_AddSayingCommand = new RelayCommand(p => AddSaying(p.ToString()));
                }
                return this.m_AddSayingCommand;
            }
        }

        #endregion

        /// <summary>
        /// 清除个人信息
        /// </summary>
        public void ClearUserData()
        {
            this.SayingList = new ObservableCollection<DoubanMiniBlog>();
            this.CurrentPeople = new DoubanPeople();
        }
    }
}