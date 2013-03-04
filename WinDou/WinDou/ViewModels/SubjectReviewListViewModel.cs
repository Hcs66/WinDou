using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using DoubanSharp.Model;
using System.ComponentModel;
using HcsLib.WindowsPhone.ViewModel;

namespace WinDou.ViewModels
{
    public class SubjectReviewListViewModel : ViewModelBase
    {
        private int m_RowPerPages = 10;
        private int m_CurrentSearchPageIndex = 0;
        public SubjectReviewListViewModel()
        {
            ReviewList = new ObservableCollection<DoubanSubjectReview>();
            this.PropertyChanged += new PropertyChangedEventHandler(SubjectReviewListViewModel_PropertyChanged);
        }

        void SubjectReviewListViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
            switch (propertyName)
            {
                case "ReviewList":
                    if (GetReviewsCompleted != null)
                    {
                        GetReviewsCompleted(this, new DoubanSearchCompletedEventArgs() { IsSuccess = true });
                    }
                    break;
                default:
                    break;
            }
        }

        #region 属性

        public string Title { get; set; }

        public ObservableCollection<DoubanSubjectReview> ReviewList { get; set; }

        public EventHandler<DoubanSearchCompletedEventArgs> GetReviewsCompleted { get; set; }

        /// <summary>
        /// 加载更多数据按钮可见性
        /// </summary>
        public Visibility LoadMoreVisibility { get; set; }

        #endregion

        public override void LoadData()
        {
        }

        public void GetReviews(string subjectId, string subjectType, bool isPaging = false)
        {
            this.IsBusy = true;
            if (isPaging)
            {
                m_CurrentSearchPageIndex += m_RowPerPages;
            }
            else
            {
                m_CurrentSearchPageIndex = 0;
                ReviewList = new ObservableCollection<DoubanSubjectReview>();
            }
            App.DoubanService.SearchSubjectReviews(subjectId, m_CurrentSearchPageIndex.ToString(), m_RowPerPages.ToString(),
                   (result, resp) =>
                   {
                       if (resp.RestResponse.StatusCode == HttpStatusCode.OK && result.ReviewList.Count > 0)
                       {
                           System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                           {
                               foreach (var item in result.ReviewList)
                               {
                                   this.ReviewList.Add(item);
                               }
                               this.Title = result.ResultTitle + "(" + result.Total + ")";//result.Title + "(" + result.TotalResults.ToString() + ")";
                               this.LoadMoreVisibility = Visibility.Collapsed;//this.ReviewList.Count < result.TotalResults ? Visibility.Visible : Visibility.Collapsed;
                               this.OnPropertyChanged("LoadMoreVisibility");
                               this.OnPropertyChanged("Title");
                               this.OnPropertyChanged("ReviewList");
                               this.IsBusy = false;
                           });
                       }
                       else if (GetReviewsCompleted != null)
                       {
                           GetReviewsCompleted(this, new DoubanSearchCompletedEventArgs() { IsSuccess = false });
                       }
                   });

        }
    }
}
