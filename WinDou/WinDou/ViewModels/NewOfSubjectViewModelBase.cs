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
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using DoubanSharp.Model;
using SocialEbola.Lib.HapHelper;
using HtmlAgilityPack;
using HcsLib.WindowsPhone.ViewModel;
using HcsLib.WindowsPhone.Msic;

namespace WinDou.ViewModels
{
    public abstract class NewOfSubjectViewModelBase : ViewModelBase
    {
        #region 属性
        public EventHandler LoadCompleted;
        protected static Regex regexRemoveBlank = new Regex("[\\n\\s]+");
        protected static Regex regexSubjetId = new Regex("/([\\d]+)/?");
        protected string SubjectListUrl { get; set; }
        #endregion

        #region 方法

        public void LoadData(bool isRefresh)
        {
            if (!isRefresh)
            {
                //读取缓存
                if (LoadCacheList())
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        //通知更新
                        NotifyOnPropertyChnged();
                        if (LoadCompleted != null)
                        {
                            LoadCompleted(this, null);
                        }
                    });
                    return;
                }
            }
            var request = (HttpWebRequest)WebRequest.Create(SubjectListUrl);
            request.BeginGetResponse(
                r =>
                {
                    var httpRequest = (HttpWebRequest)r.AsyncState;
                    var httpResponse = (HttpWebResponse)httpRequest.EndGetResponse(r);
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var reader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            string content = reader.ReadToEnd();
                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(content);
                            //生成Subject列表
                            BuildSubjectList(doc.DocumentNode);
                            //缓存
                            SaveCacheList();
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                //通知更新
                                NotifyOnPropertyChnged();
                                if (LoadCompleted != null)
                                {
                                    LoadCompleted(this, null);
                                }
                            });
                        }
                    }
                }, request);
        }

        public override void LoadData()
        {
            LoadData(false);
        }

        protected List<DoubanSubject> GetCacheList(string cacheFileName)
        {
            List<DoubanSubject> cacheList = IsolatedStorageHelper.LoadFile<List<DoubanSubject>>(cacheFileName, true);
            if (cacheList != null)
            {
                return cacheList;
            }
            return new List<DoubanSubject>();
        }

        //构建Subject列表
        protected abstract void BuildSubjectList(HtmlNode root);
        //将html转换为Subject
        protected abstract List<DoubanSubject> ParseSubjecList(IEnumerable<HtmlAbstractor> liList);
        //通知更新
        protected abstract void NotifyOnPropertyChnged();
        //加载缓存
        protected abstract bool LoadCacheList();
        //缓存列表
        protected abstract void SaveCacheList();
        #endregion

    }
}
