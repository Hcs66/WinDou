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
using DoubanSharp.Model;
using System.IO;
using System.Text.RegularExpressions;
//using DoubanSharp.Model.Enum;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml;
using HcsLib.WindowsPhone.ViewModel;

namespace WinDou.ViewModels
{
    public class SubjectReviewViewModel : ViewModelBase
    {
        private int m_MaxLengthPerPage = 800;
        public SubjectReviewViewModel()
        { }

        #region 属性

        public EventHandler<DoubanSearchCompletedEventArgs> GetReviewCompleted;
        public int TotalPages { get; set; }
        public List<string> ReveiwContentList;
        #endregion

        #region 方法


        public override void LoadData()
        {

        }

        public void GetReview(string reviewId)
        {
            App.DoubanService.GetReview(reviewId,
                (review, resp) =>
                {
                    if (resp.RestResponse.StatusCode == HttpStatusCode.OK)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            review.SubjectTitle = "评论：" + review.SubjectTitle;
                            ReveiwContentList = new List<string>();
                            TotalPages = review.Content.Length / m_MaxLengthPerPage;
                            int startIndex = 0;
                            for (int i = 0; i < TotalPages; i++)
                            {
                                startIndex = i * m_MaxLengthPerPage;
                                ReveiwContentList.Add(review.Content.Substring(startIndex, m_MaxLengthPerPage));
                            }
                            if (review.Content.Length % m_MaxLengthPerPage > 0)
                            {
                                ReveiwContentList.Add(review.Content.Substring(TotalPages * m_MaxLengthPerPage));
                            }
                            TotalPages = ReveiwContentList.Count;
                            if (GetReviewCompleted != null)
                            {
                                GetReviewCompleted(this, new DoubanSearchCompletedEventArgs() { IsSuccess = true, Message = "", Result = review });
                            }
                        });
                    }
                    else if (GetReviewCompleted != null)
                    {
                        GetReviewCompleted(this, new DoubanSearchCompletedEventArgs() { IsSuccess = resp.RestResponse.StatusCode == HttpStatusCode.OK });
                    }
                }
                );
        }
        #endregion
    }
}
