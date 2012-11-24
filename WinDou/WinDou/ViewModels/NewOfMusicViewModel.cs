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
    public class NewOfMusicViewModel : NewOfSubjectViewModelBase
    {

        #region 属性
        public List<DoubanSubject> ChinaList { get; set; }
        public List<DoubanSubject> WesternList { get; set; }
        public List<DoubanSubject> EasternList { get; set; }
        #endregion

        public NewOfMusicViewModel()
        {
            this.SubjectListUrl = "http://music.douban.com/";
        }

        protected override void BuildSubjectList(HtmlNode root)
        {
            ChinaList = ParseSubjecList(root.SelectNodes("//ul[@id='newcontent1']//li"));
            WesternList = ParseSubjecList(root.SelectNodes("//ul[@id='newcontent2']//li"));
            EasternList = ParseSubjecList(root.SelectNodes("//ul[@id='newcontent3']//li"));
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
            this.OnPropertyChanged("ChinaList");
            this.OnPropertyChanged("WesternList");
            this.OnPropertyChanged("EasternList");
        }

        protected override bool LoadCacheList()
        {
            ChinaList = GetCacheList(Globals.MUSIC_CHINALIST_FILENAME);
            WesternList = GetCacheList(Globals.MUSIC_WESTERNLIST_FILENAME);
            EasternList = GetCacheList(Globals.MUSIC_EASTERNLIST_FILENAME);
            return ChinaList.Count > 0;
        }

        protected override void SaveCacheList()
        {
            IsolatedStorageHelper.SaveFile<List<DoubanSubject>>(Globals.MUSIC_CHINALIST_FILENAME, ChinaList, true);
            IsolatedStorageHelper.SaveFile<List<DoubanSubject>>(Globals.MUSIC_WESTERNLIST_FILENAME, WesternList, true);
            IsolatedStorageHelper.SaveFile<List<DoubanSubject>>(Globals.MUSIC_EASTERNLIST_FILENAME, EasternList, true);        }
    }
}
