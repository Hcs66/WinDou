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
    public class NewOfMusicViewModel : NewOfSubjectViewModelBase<DoubanMusic>
    {

        #region 属性
        public List<DoubanMusic> ChinaList { get; set; }
        public List<DoubanMusic> WesternList { get; set; }
        public List<DoubanMusic> EasternList { get; set; }
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

        protected override List<DoubanMusic> ParseSubjecList(IEnumerable<HtmlAbstractor> liList)
        {
            List<DoubanMusic> subjectList = new List<DoubanMusic>();
            try
            {
                foreach (var li in liList)
                {
                    HtmlNodeCollection fictionNodes = (li as HtmlElementAbstractor).Element.ChildNodes;
                    if (fictionNodes.Count == 0)
                    {
                        continue;
                    }
                    //标题和链接
                    HtmlNode h3 = fictionNodes.FindFirst("h3");
                    HtmlNode titleNode = h3.LastChild;
                    //作者和简介
                    HtmlNode authorNode = h3.NextSibling;
                    string author = "";
                    if (authorNode != null)
                    {
                        author = authorNode.InnerText.Replace("\n", "").Replace(" ", "");
                    }
                    //图片
                    HtmlNode img = fictionNodes.FindFirst("img");
                    subjectList.Add(new DoubanMusic()
                    {
                        Id = regexSubjetId.Match(titleNode.Attributes["href"].Value).Groups[1].Value,
                        AuthorName = author,
                        Author = new List<DoubanAuthor>() { new DoubanAuthor() { Name = author } },
                        Summary = "",
                        Title = regexRemoveBlank.Replace(titleNode.InnerText, ""),
                        Image = img.Attributes["src"].Value
                    });
                }
            }
            catch
            {
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
            IsolatedStorageHelper.SaveFile<List<DoubanMusic>>(Globals.MUSIC_CHINALIST_FILENAME, ChinaList);
            IsolatedStorageHelper.SaveFile<List<DoubanMusic>>(Globals.MUSIC_WESTERNLIST_FILENAME, WesternList);
            IsolatedStorageHelper.SaveFile<List<DoubanMusic>>(Globals.MUSIC_EASTERNLIST_FILENAME, EasternList);
        }
    }
}
