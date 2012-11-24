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
using DoubanSharp.Model.Enum;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml;
using HcsLib.WindowsPhone.ViewModel;

namespace WinDou.ViewModels
{
    public class SubjectReviewViewModel : ViewModelBase
    {
        private static Regex m_HtmlTagRegex = new Regex(@"</?[a-z][a-z0-9]*[^<>]*>");
        private static Regex m_ReviewContentRegex = new Regex("<span property=\"v:description\">([\\s\\S]+?)</span>");
        private static Regex m_ReviewItemRegex = new Regex("<span property=\"v:itemreviewed\">([\\s\\S]+?)</span>");
        private int m_MaxLengthPerPage = 500;
        public SubjectReviewViewModel()
        { }

        #region 属性

        public EventHandler<DoubanSearchCompletedEventArgs> GetReviewCompleted;
        public EventHandler GetReviewContentCompleted;
        public string CurrentContent { get; set; }
        public int CurrentPageIndex { get; set; }
        public int TotalPages { get; set; }
        public DoubanReview CurrentReview { get; set; }
        public string CurrentReviewId { get; set; }
        public List<string> ReveiwContentList;
        #endregion

        #region 方法


        public override void LoadData()
        {

        }

        public void GetReview(string reviewId)
        {
            App.DoubanService.GetReview(reviewId,
                (obj, resp) =>
                {
                    DoubanReview review = new DoubanReview();
                    if (resp.StatusCode == HttpStatusCode.OK)
                    {                     
                        review = obj;
                        review.Rating.Average = review.Rating.Value;
                        review.RawSource = review.Summary;
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            this.CurrentReview = review;
                            this.CurrentReviewId = reviewId;
                            if (GetReviewCompleted != null)
                            {
                                GetReviewCompleted(this, new DoubanSearchCompletedEventArgs() { IsSuccess = true, Message = "", Result = review });
                            }
                        });
                    }
                    else if (GetReviewCompleted != null)
                    {
                        GetReviewCompleted(this, new DoubanSearchCompletedEventArgs() { IsSuccess = resp.StatusCode == HttpStatusCode.OK, Message = resp.InnerException.Message });

                    }
                }
                );
        }

        private void FetchReviewContent(DoubanReview review, string reviewId)
        {

            var feedsRequest = (HttpWebRequest)WebRequest.Create("http://www.douban.com/review/" + reviewId + "/");
            feedsRequest.BeginGetResponse(r =>
            {
                var httpRequest = (HttpWebRequest)r.AsyncState;
                var httpResponse = (HttpWebResponse)httpRequest.EndGetResponse(r);
                using (var reader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string content = reader.ReadToEnd();
                    //匹配评论项标题
                    review.Subject.Title = "《" + m_ReviewItemRegex.Match(content).Groups[1].Value + "》";
                    //匹配评论内容
                    content = m_ReviewContentRegex.Match(content).Groups[1].Value;
                    //去除html标记
                    review.RawSource = m_HtmlTagRegex.Replace(content, "");
                    //设置翻页信息
                    CurrentPageIndex = 0;
                    ReveiwContentList = new List<string>();
                    TotalPages = review.RawSource.Length / m_MaxLengthPerPage;
                    int startIndex = 0;
                    for (int i = 0; i < TotalPages; i++)
                    {
                        startIndex = i * m_MaxLengthPerPage;
                        ReveiwContentList.Add(review.RawSource.Substring(startIndex, m_MaxLengthPerPage));
                    }
                    if (review.RawSource.Length % m_MaxLengthPerPage > 0)
                    {
                        ReveiwContentList.Add(review.RawSource.Substring(TotalPages * m_MaxLengthPerPage));
                    }
                    TotalPages = ReveiwContentList.Count;
                    //置为第一页
                    review.RawSource = ReveiwContentList[0];
                    if (GetReviewCompleted != null)
                    {
                        GetReviewCompleted(this, new DoubanSearchCompletedEventArgs() { IsSuccess = true, Message = "", Result = review });
                    }
                }

            }, feedsRequest);
        }

        public void FetchReviewContent()
        {

            var feedsRequest = (HttpWebRequest)WebRequest.Create("http://www.douban.com/review/" + this.CurrentReviewId + "/");
            feedsRequest.BeginGetResponse(r =>
            {
                var httpRequest = (HttpWebRequest)r.AsyncState;
                var httpResponse = (HttpWebResponse)httpRequest.EndGetResponse(r);
                using (var reader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string content = reader.ReadToEnd();
                    //匹配评论内容
                    content = m_ReviewContentRegex.Match(content).Groups[1].Value;
                    //去除html标记
                    content = m_HtmlTagRegex.Replace(content, "");
                    //设置翻页信息
                    CurrentPageIndex = 0;
                    ReveiwContentList = new List<string>();
                    TotalPages = content.Length / m_MaxLengthPerPage;
                    int startIndex = 0;
                    for (int i = 0; i < TotalPages; i++)
                    {
                        startIndex = i * m_MaxLengthPerPage;
                        ReveiwContentList.Add(content.Substring(startIndex, m_MaxLengthPerPage));
                    }
                    if (content.Length % m_MaxLengthPerPage > 0)
                    {
                        ReveiwContentList.Add(content.Substring(TotalPages * m_MaxLengthPerPage));
                    }
                    TotalPages = ReveiwContentList.Count;
                    if (GetReviewContentCompleted != null)
                    {
                        GetReviewContentCompleted(this, null);
                    }
                }

            }, feedsRequest);
        }
        #endregion
    }
}
