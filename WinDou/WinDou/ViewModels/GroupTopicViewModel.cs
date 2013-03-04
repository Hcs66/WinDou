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
    public class GroupTopicViewModel : ViewModelBase
    {
        private int m_MaxLengthPerPage = 800;
        private int m_RowPerPages = 50;
        private int m_CurrentPageIndex = 0;
        private int m_TotalReview = 0;
        public GroupTopicViewModel()
        {
            CurrentTopic = new DoubanGroupTopic();
            this.PropertyChanged += new PropertyChangedEventHandler(GroupTopicViewModel_PropertyChanged);
        }

        void GroupTopicViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
            switch (propertyName)
            {
                case "GroupTopicReviewList":
                    if (GetGroupTopicReviewListCompleted != null)
                    {
                        GetGroupTopicReviewListCompleted(this, new DoubanSearchCompletedEventArgs() { IsSuccess = true, Result = m_TotalReview });
                    }
                    break;
                case "CurrentTopic":
                    if (GetGroupTopicCompleted != null)
                    {
                        GetGroupTopicCompleted(this, new DoubanSearchCompletedEventArgs() { IsSuccess = true });
                    }
                    break;
                default:
                    break;
            }
        }

        #region 属性

        public EventHandler<DoubanSearchCompletedEventArgs> GetGroupTopicCompleted { get; set; }

        public List<string> TopicContentList { get; set; }

        public DoubanGroupTopic CurrentTopic { get; set; }

        public ObservableCollection<DoubanGroupTopicReview> GroupTopicReviewList { get; set; }

        public EventHandler<DoubanSearchCompletedEventArgs> GetGroupTopicReviewListCompleted { get; set; }



        /// <summary>
        /// 加载更多数据按钮可见性
        /// </summary>
        public Visibility LoadMoreVisibility { get; set; }

        #endregion
        public override void LoadData()
        {
        }

        public void GetGroupTopic(string tid)
        {
            App.DoubanService.GetGroupTopic(tid,
                  (topic, resp) =>
                  {
                      if (resp.RestResponse.StatusCode == HttpStatusCode.OK)
                      {
                          Deployment.Current.Dispatcher.BeginInvoke(() =>
                          {
                              CurrentTopic = topic;
                              CurrentTopic.Author.Name = "来自：" + CurrentTopic.Author.Name;
                              TopicContentList = new List<string>();
                              int totalPages = CurrentTopic.Content.Length / m_MaxLengthPerPage;
                              int startIndex = 0;
                              for (int i = 0; i < totalPages; i++)
                              {
                                  startIndex = i * m_MaxLengthPerPage;
                                  TopicContentList.Add(CurrentTopic.Content.Substring(startIndex, m_MaxLengthPerPage));
                              }
                              if (CurrentTopic.Content.Length % m_MaxLengthPerPage > 0)
                              {
                                  TopicContentList.Add(CurrentTopic.Content.Substring(totalPages * m_MaxLengthPerPage));
                              }
                              this.OnPropertyChanged("CurrentTopic");
                          });
                      }
                  }
                  );
        }

        public void GetGroupTopicReviews(string tid, bool isPaging = false)
        {
            if (isPaging)
            {
                m_CurrentPageIndex += m_RowPerPages;
            }
            else
            {
                m_CurrentPageIndex = 0;
                this.GroupTopicReviewList = new ObservableCollection<DoubanGroupTopicReview>();
            }
            App.DoubanService.SearchGroupTopicReviews(tid, m_CurrentPageIndex.ToString(), m_RowPerPages.ToString(),
                (results, resp) =>
                {
                    if (resp.RestResponse.StatusCode == HttpStatusCode.OK)
                    {
                        if (results.Comments.Count <= 0)
                        {
                            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                m_CurrentPageIndex = m_CurrentPageIndex > m_RowPerPages ? m_CurrentPageIndex - m_RowPerPages : 1;
                                this.LoadMoreVisibility = Visibility.Collapsed;
                                this.OnPropertyChanged("LoadMoreVisibility");
                                if (GetGroupTopicReviewListCompleted != null)
                                {
                                    GetGroupTopicReviewListCompleted(null, new DoubanSearchCompletedEventArgs() { IsSuccess = false, Message = "没有更多的评论了" });
                                }
                            });
                        }
                        else if (results.Comments != null && results.Comments.Count > 0)
                        {
                            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                foreach (var item in results.Comments)
                                {
                                    this.GroupTopicReviewList.Add(item);
                                }
                                m_TotalReview = int.Parse(results.Total);
                                this.LoadMoreVisibility = this.GroupTopicReviewList.Count < m_TotalReview ? Visibility.Visible : Visibility.Collapsed;
                                this.OnPropertyChanged("LoadMoreVisibility");
                                this.OnPropertyChanged("GroupTopicReviewList");
                            });
                        }
                    }
                    else//出错后序号置为原值
                    {
                        m_CurrentPageIndex = m_CurrentPageIndex > m_RowPerPages ? m_CurrentPageIndex - m_RowPerPages : 1;
                        if (GetGroupTopicReviewListCompleted != null)
                        {
                            GetGroupTopicReviewListCompleted(null, new DoubanSearchCompletedEventArgs() { IsSuccess = false, Message = "加载出错，请重试" });
                        }
                    }
                });
        }

        public void LoadMoreReviews()
        {
            GetGroupTopicReviews(this.CurrentTopic.Id, true);
        }
    }
}
