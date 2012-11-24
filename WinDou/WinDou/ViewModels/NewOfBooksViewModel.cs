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
using HcsLib.WindowsPhone.Msic;


namespace WinDou.ViewModels
{
    public class NewOfBooksViewModel : NewOfSubjectViewModelBase
    {
        #region 属性
        public List<DoubanSubject> FictionList { get; set; }
        public List<DoubanSubject> LiteratureList { get; set; }
        #endregion

        #region 方法

        public NewOfBooksViewModel()
        {
            this.SubjectListUrl = "http://book.douban.com/latest";
        }

        protected override void BuildSubjectList(HtmlNode root)
        {
            FictionList = ParseSubjecList(root.SelectNodes("//div[@class='article']//ul//li"));
            LiteratureList = ParseSubjecList(root.SelectNodes("//div[@class='aside']//ul//li"));
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
                //标题
                HtmlNode h2 = fictionNodes.FindFirst("h2");
                //作者和简介
                IEnumerable<HtmlNode> pArr = fictionNodes.Elements("p");
                int index = 0;
                string[] authorDescArr = new string[2];
                foreach (var p in pArr)
                {
                    authorDescArr[index] = regexRemoveBlank.Replace(p.InnerText, "");
                    index++;
                }
                //链接
                HtmlNode a = fictionNodes.FindFirst("a");
                //图片
                HtmlNode img = fictionNodes.FindFirst("img");
                subjectList.Add(new DoubanSubject()
                {
                    Id = regexSubjetId.Match(a.Attributes["href"].Value).Groups[1].Value,
                    Author = new DoubanAuthor() { AuthorName = authorDescArr[0] },
                    Summary = authorDescArr[1],
                    Title = regexRemoveBlank.Replace(h2.InnerText, ""),
                    Links = new List<DoubanLink>() { new DoubanLink() { Href = img.Attributes["src"].Value } }
                });
            }
            return subjectList;
        }

        protected override void NotifyOnPropertyChnged()
        {
            this.OnPropertyChanged("FictionList");
            this.OnPropertyChanged("LiteratureList");
        }

        protected override bool LoadCacheList()
        {
            FictionList = GetCacheList(Globals.BOOK_FICTIONLIST_FILENAME);
            LiteratureList = GetCacheList(Globals.BOOK_LITERATURELIST_FILENAME);
            return FictionList.Count > 0;
        }

        protected override void SaveCacheList()
        {
            IsolatedStorageHelper.SaveFile<List<DoubanSubject>>(Globals.BOOK_FICTIONLIST_FILENAME, FictionList, true);
            IsolatedStorageHelper.SaveFile<List<DoubanSubject>>(Globals.BOOK_LITERATURELIST_FILENAME, LiteratureList, true);
        }

        #endregion
    }
}
