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
using System.Collections.Generic;
using System.IO;
using DoubanSharp.Model;
using HtmlAgilityPack;
using SocialEbola.Lib.HapHelper;
using System.Text.RegularExpressions;
using System.Text;
using HcsLib.WindowsPhone.Msic;


namespace WinDou.ViewModels
{
    public class NewOfMoviesViewModel : NewOfSubjectViewModelBase
    {

        #region 属性
        public List<DoubanSubject> AllList { get; set; }
        public List<DoubanSubject> ActionList { get; set; }
        public List<DoubanSubject> PlotList { get; set; }
        public List<DoubanSubject> ComedyList { get; set; }
        public List<DoubanSubject> SuspenseList { get; set; }
        #endregion

        public NewOfMoviesViewModel()
        {
            this.SubjectListUrl = "http://movie.douban.com/";
        }

        protected override void BuildSubjectList(HtmlNode root)
        {
            AllList = ParseSubjecList(root.SelectNodes("//ul[@id='newcontent1']//li"));
            ActionList = ParseSubjecList(root.SelectNodes("//ul[@id='newcontent2']//li"));
            PlotList = ParseSubjecList(root.SelectNodes("//ul[@id='newcontent3']//li"));
            ComedyList = ParseSubjecList(root.SelectNodes("//ul[@id='newcontent4']//li"));
            SuspenseList = ParseSubjecList(root.SelectNodes("//ul[@id='newcontent5']//li"));
        }

        protected override List<DoubanSubject> ParseSubjecList(IEnumerable<HtmlAbstractor> liList)
        {
            List<DoubanSubject> subjectList = new List<DoubanSubject>();
            foreach (var li in liList)
            {
                HtmlNodeCollection fictionNodes = (li as HtmlElementAbstractor).Element.ChildNodes;
                if (fictionNodes.Count == 0)
                {
                    continue;
                }
                //标题和链接
                HtmlNode titleNode = fictionNodes.FindFirst("h3").LastChild;
                //作者和简介
                IEnumerable<HtmlNode> authorDescArr = fictionNodes.Elements("span");
                StringBuilder authorDescString = new StringBuilder();
                foreach (var item in authorDescArr)
                {
                    authorDescString.Append(regexRemoveBlank.Replace(item.InnerText, ""));
                    if (item.NextSibling.NodeType == HtmlNodeType.Text)
                    {
                        authorDescString.Append(regexRemoveBlank.Replace(item.NextSibling.InnerText, "")).Append("/");
                    }

                }
                //图片
                HtmlNode img = fictionNodes.FindFirst("img");
                subjectList.Add(new DoubanSubject()
                {
                    Id = regexSubjetId.Match(titleNode.Attributes["href"].Value).Groups[1].Value,
                    Author = new DoubanAuthor() { AuthorName = authorDescString.ToString().TrimEnd('/') },
                    Summary = "",
                    Title = regexRemoveBlank.Replace(titleNode.InnerText, ""),
                    Links = new List<DoubanLink>() { new DoubanLink() { Href = img.Attributes["src"].Value } }
                });
            }
            return subjectList;
        }

        protected override void NotifyOnPropertyChnged()
        {
            this.OnPropertyChanged("AllList");
            this.OnPropertyChanged("ActionList");
            this.OnPropertyChanged("PlotList");
            this.OnPropertyChanged("ComedyList");
            this.OnPropertyChanged("SuspenseList");
        }

        protected override bool LoadCacheList()
        {
            AllList = GetCacheList(Globals.MOVIE_ALLLIST_FILENAME);
            ActionList = GetCacheList(Globals.MOVIE_ACTIONLIST_FILENAME);
            PlotList = GetCacheList(Globals.MOVIE_PLOTLIST_FILENAME);
            ComedyList = GetCacheList(Globals.MOVIE_COMEDYLIST_FILENAME);
            SuspenseList = GetCacheList(Globals.MOVIE_SUSPENSELIST_FILENAME);
            return AllList.Count > 0;
        }

        protected override void SaveCacheList()
        {
            IsolatedStorageHelper.SaveFile<List<DoubanSubject>>(Globals.MOVIE_ALLLIST_FILENAME, AllList, true);
            IsolatedStorageHelper.SaveFile<List<DoubanSubject>>(Globals.MOVIE_ACTIONLIST_FILENAME, ActionList, true);
            IsolatedStorageHelper.SaveFile<List<DoubanSubject>>(Globals.MOVIE_PLOTLIST_FILENAME, PlotList, true);
            IsolatedStorageHelper.SaveFile<List<DoubanSubject>>(Globals.MOVIE_COMEDYLIST_FILENAME, ComedyList, true);
            IsolatedStorageHelper.SaveFile<List<DoubanSubject>>(Globals.MOVIE_SUSPENSELIST_FILENAME, SuspenseList, true);
        }
    }
}
