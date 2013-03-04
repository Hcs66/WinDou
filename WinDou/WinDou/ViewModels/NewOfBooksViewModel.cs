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
    public class NewOfBooksViewModel : NewOfSubjectViewModelBase<DoubanBook>
    {
        #region 属性
        public List<DoubanBook> FictionList { get; set; }
        public List<DoubanBook> LiteratureList { get; set; }
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

        protected override List<DoubanBook> ParseSubjecList(IEnumerable<HtmlAbstractor> liList)
        {
            List<DoubanBook> subjectList = new List<DoubanBook>();
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
                subjectList.Add(new DoubanBook()
                {
                    Id = regexSubjetId.Match(a.Attributes["href"].Value).Groups[1].Value,
                    AuthorName = authorDescArr[0],
                    Author = new List<string>() { authorDescArr[0] },
                    Summary = authorDescArr[1],
                    Title = regexRemoveBlank.Replace(h2.InnerText, ""),
                    Image = img.Attributes["src"].Value
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
            IsolatedStorageHelper.SaveFile<List<DoubanBook>>(Globals.BOOK_FICTIONLIST_FILENAME, FictionList);
            IsolatedStorageHelper.SaveFile<List<DoubanBook>>(Globals.BOOK_LITERATURELIST_FILENAME, LiteratureList);
        }

        #endregion
    }
}
