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
    public class GroupViewModel : ViewModelBase
    {
        private int m_MaxLengthPerPage = 800;
        private int m_RowPerPages = 50;
        private int m_CurrentTopicsPageIndex = 0;
        public GroupViewModel()
        {
            CurrentGroupTopicList = new ObservableCollection<DoubanGroupTopic>();
            this.PropertyChanged += new PropertyChangedEventHandler(GroupViewModel_PropertyChanged);
        }

        void GroupViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
            switch (propertyName)
            {
                case "CurrentGroupTopicList":
                    if (GetGroupTopicsCompleted != null)
                    {
                        GetGroupTopicsCompleted(this, new DoubanSearchCompletedEventArgs() { IsSuccess = true });
                    }
                    break;
                case "CurrentGroup":
                    if (GetGroupCompleted != null)
                    {
                        GetGroupCompleted(this, new EventArgs());
                    }
                    break;
                default:
                    break;
            }
        }

        #region 属性

        public EventHandler<DoubanSearchCompletedEventArgs> GetGroupTopicsCompleted { get; set; }

        public EventHandler GetGroupCompleted { get; set; }

        public DoubanGroup CurrentGroup { get; set; }

        public ObservableCollection<DoubanGroupTopic> CurrentGroupTopicList { get; set; }

        public List<string> GroupContentList { get; set; }

        /// <summary>
        /// 加载更多数据按钮可见性
        /// </summary>
        public Visibility LoadMoreVisibility { get; set; }

        #endregion
        public override void LoadData()
        {
        }

        public void GetGroup(string uid)
        {
            App.DoubanService.GetGroup(uid,
                (group, resp) =>
                {
                    System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        CurrentGroup = group;
                        int totalPages = CurrentGroup.Desc.Length / m_MaxLengthPerPage;
                        int startIndex = 0;
                        GroupContentList = new List<string>();
                        for (int i = 0; i < totalPages; i++)
                        {
                            startIndex = i * m_MaxLengthPerPage;
                            GroupContentList.Add(CurrentGroup.Desc.Substring(startIndex, m_MaxLengthPerPage));
                        }
                        if (CurrentGroup.Desc.Length % m_MaxLengthPerPage > 0)
                        {
                            GroupContentList.Add(CurrentGroup.Desc.Substring(totalPages * m_MaxLengthPerPage));
                        }
                        this.OnPropertyChanged("CurrentGroup");
                    });
                });
        }

        public void GetGroupTopics(string uid, bool isPaging = false)
        {
            if (isPaging)
            {
                m_CurrentTopicsPageIndex += m_RowPerPages;

            }
            else
            {
                m_CurrentTopicsPageIndex = 0;
                this.CurrentGroupTopicList = new ObservableCollection<DoubanGroupTopic>();
            }
            App.DoubanService.SearchGroupTopics(uid, m_CurrentTopicsPageIndex.ToString(), m_RowPerPages.ToString(),
                     (results, resp) =>
                     {
                         if (resp.RestResponse.StatusCode == HttpStatusCode.OK)
                         {
                             if (results.Topics.Count <= 0)
                             {
                                 System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                                 {
                                     m_CurrentTopicsPageIndex = m_CurrentTopicsPageIndex > m_RowPerPages ? m_CurrentTopicsPageIndex - m_RowPerPages : 1;
                                     if (GetGroupTopicsCompleted != null)
                                     {
                                         GetGroupTopicsCompleted(null, new DoubanSearchCompletedEventArgs() { IsSuccess = false, Message = "没有更多的话题了" });
                                     }
                                 });
                             }
                             else if (results.Topics != null && results.Topics.Count > 0)
                             {
                                 System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                                 {
                                     foreach (var item in results.Topics)
                                     {
                                         item.CommentsCount += "回应";
                                         this.CurrentGroupTopicList.Add(item);
                                     }
                                     //缓存
                                     this.OnPropertyChanged("CurrentGroupTopicList");
                                 });
                             }
                         }
                         else//出错后序号置为原值
                         {
                             m_CurrentTopicsPageIndex = m_CurrentTopicsPageIndex > m_RowPerPages ? m_CurrentTopicsPageIndex - m_RowPerPages : 1;
                             if (GetGroupTopicsCompleted != null)
                             {
                                 GetGroupTopicsCompleted(null, new DoubanSearchCompletedEventArgs() { IsSuccess = false, Message = "加载出错，请重试" });
                             }
                         }
                     }
                 );
        }
    }
}
