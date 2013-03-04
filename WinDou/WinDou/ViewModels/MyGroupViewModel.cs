using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DoubanSharp.Model;
using HcsLib.WindowsPhone.ViewModel;

namespace WinDou.ViewModels
{
    public class MyGroupViewModel : ViewModelBase
    {
        private int m_RowPerPages = 50;
        private int m_AllTopicPageIndex = 0;
        private int m_CreateTopicPageIndex = 0;
        private int m_ReplyTopicPageIndex = 0;

        public MyGroupViewModel()
        {
            AllTopicList = new ObservableCollection<DoubanGroupTopic>();
            CreateTopicList = new ObservableCollection<DoubanGroupTopic>();
            ReplyTopicList = new ObservableCollection<DoubanGroupTopic>();
            this.PropertyChanged += new PropertyChangedEventHandler(GroupViewModel_PropertyChanged);
        }

        void GroupViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
            switch (propertyName)
            {
                case "AllTopicList":
                    if (GetAllTopicsCompleted != null)
                    {
                        GetAllTopicsCompleted(this, new DoubanSearchCompletedEventArgs() { IsSuccess = true });
                    }
                    break;
                case "CreateTopicList":
                    if (GetCreateTopicsCompleted != null)
                    {
                        GetCreateTopicsCompleted(this, new DoubanSearchCompletedEventArgs() { IsSuccess = true });
                    }
                    break;
                case "ReplyTopicList":
                    if (GetReplyTopicsCompleted != null)
                    {
                        GetReplyTopicsCompleted(this, new DoubanSearchCompletedEventArgs() { IsSuccess = true });
                    }
                    break;
                default:
                    break;
            }
        }

        #region 属性
        public ObservableCollection<DoubanGroupTopic> AllTopicList { get; set; }

        public EventHandler<DoubanSearchCompletedEventArgs> GetAllTopicsCompleted { get; set; }

        public ObservableCollection<DoubanGroupTopic> CreateTopicList { get; set; }

        public EventHandler<DoubanSearchCompletedEventArgs> GetCreateTopicsCompleted { get; set; }

        public ObservableCollection<DoubanGroupTopic> ReplyTopicList { get; set; }

        public EventHandler<DoubanSearchCompletedEventArgs> GetReplyTopicsCompleted { get; set; }

        /// <summary>
        /// 加载更多数据按钮可见性
        /// </summary>
        public Visibility AllTopicLoadMoreVisibility { get; set; }

        public Visibility CreateTopicLoadMoreVisibility { get; set; }

        public Visibility ReplyTopicLoadMoreVisibility { get; set; }

        #endregion
        public override void LoadData()
        {
        }

        private void GetTopics(DoubanGroupTopicSearch result, DoubanResponse resp,
            string listName, ObservableCollection<DoubanGroupTopic> list,
            string loadMoreName,
            EventHandler<DoubanSearchCompletedEventArgs> completedEvent)
        {
            if (resp.RestResponse.StatusCode == HttpStatusCode.OK && result.Topics.Count > 0)
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    foreach (var item in result.Topics)
                    {
                        item.CommentsCount = item.CommentsCount + "回应";
                        list.Add(item);
                    }
                    int total = !string.IsNullOrEmpty(result.Total) ? int.Parse(result.Total) : 0;

                    Visibility loadMore = (list.Count < total || result.HasMore) ? Visibility.Visible : Visibility.Collapsed;
                    SetLoadMoreVisibility(loadMoreName, loadMore);
                    this.OnPropertyChanged(listName);
                });
            }
            else if (completedEvent != null)
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                 {
                     SetLoadMoreVisibility(loadMoreName, Visibility.Collapsed);
                     completedEvent(this, new DoubanSearchCompletedEventArgs() { IsSuccess = false });
                 });
            }
        }

        private void SetLoadMoreVisibility(string loadMoreName, Visibility loadMore)
        {
            switch (loadMoreName)
            {
                case "AllTopicLoadMoreVisibility":
                    AllTopicLoadMoreVisibility = loadMore;
                    break;
                case "CreateTopicLoadMoreVisibility":
                    CreateTopicLoadMoreVisibility = loadMore;
                    break;
                case "ReplyTopicLoadMoreVisibility":
                    ReplyTopicLoadMoreVisibility = loadMore;
                    break;
                default:
                    break;
            }
            this.OnPropertyChanged(loadMoreName);
        }

        public void GetAllTopics(bool isPaging = false)
        {
            if (isPaging)
            {
                m_AllTopicPageIndex += m_RowPerPages;
            }
            else
            {
                m_AllTopicPageIndex = 0;
                AllTopicList = new ObservableCollection<DoubanGroupTopic>();
            }
            App.DoubanService.SearchMyGroupTopics(
                   (result, resp) =>
                   {
                       GetTopics(result, resp, "AllTopicList", AllTopicList,
                           "AllTopicLoadMoreVisibility",
                           GetAllTopicsCompleted);
                   }, m_AllTopicPageIndex.ToString(), m_RowPerPages.ToString());
        }

        public void GetCreateTopics(bool isPaging = false)
        {
            if (isPaging)
            {
                m_CreateTopicPageIndex += m_RowPerPages;
            }
            else
            {
                m_CreateTopicPageIndex = 0;
                CreateTopicList = new ObservableCollection<DoubanGroupTopic>();
            }
            App.DoubanService.SearchMyCreateGroupTopics(
                   (result, resp) =>
                   {
                       GetTopics(result, resp, "CreateTopicList", CreateTopicList,
                           "CreateTopicLoadMoreVisibility",
                           GetCreateTopicsCompleted);
                   }, m_CreateTopicPageIndex.ToString(), m_RowPerPages.ToString());
        }

        public void GetReplyTopics(bool isPaging = false)
        {
            if (isPaging)
            {
                m_ReplyTopicPageIndex += m_RowPerPages;
            }
            else
            {
                m_ReplyTopicPageIndex = 0;
                ReplyTopicList = new ObservableCollection<DoubanGroupTopic>();
            }
            App.DoubanService.SearchMyReplyGroupTopics(
                   (result, resp) =>
                   {
                       GetTopics(result, resp, "ReplyTopicList", ReplyTopicList,
                           "ReplyTopicLoadMoreVisibility",
                           GetReplyTopicsCompleted);
                   }, m_ReplyTopicPageIndex.ToString(), m_RowPerPages.ToString());
        }
    }
}
