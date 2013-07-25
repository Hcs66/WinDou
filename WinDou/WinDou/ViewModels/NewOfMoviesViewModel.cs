using System.Collections.Generic;
using DoubanSharp.Model;
using HtmlAgilityPack;
using SocialEbola.Lib.HapHelper;
using HcsLib.WindowsPhone.Msic;
using System.Linq;


namespace WinDou.ViewModels
{
    public class NewOfMoviesViewModel : NewOfSubjectViewModelBase<DoubanMovie>
    {

        #region 属性
        public List<DoubanMovie> AllList { get; set; }
        public List<DoubanMovie> ActionList { get; set; }
        public List<DoubanMovie> PlotList { get; set; }
        public List<DoubanMovie> ComedyList { get; set; }
        public List<DoubanMovie> SuspenseList { get; set; }
        #endregion

        public NewOfMoviesViewModel()
        {
            this.SubjectListUrl = "http://movie.douban.com/chart";
        }

        protected override void BuildSubjectList(HtmlNode root)
        {
            AllList = ParseSubjecList(root.SelectNodes("//div[@class='indent']//table"));
        }

        protected override List<DoubanMovie> ParseSubjecList(IEnumerable<HtmlAbstractor> itemList)
        {
            List<DoubanMovie> subjectList = new List<DoubanMovie>();
            try
            {


                foreach (var item in itemList)
                {
                    HtmlNodeCollection movieNodes = (item as HtmlElementAbstractor).Element.ChildNodes;
                    if (movieNodes.Count == 0)
                    {
                        continue;
                    }
                    //标题和链接
                    HtmlNode titleNode = movieNodes.FindFirst("a");
                    string title = titleNode.Attributes["title"].Value;
                    //图片
                    HtmlNode imgNode = titleNode.Element("img");

                    HtmlNode infoNode = movieNodes.FindFirst("p");
                    HtmlNodeCollection infoNodeChilds = infoNode.ChildNodes;
                    //班底
                    string meta = infoNode.InnerText;
                    string desc = "";
                    subjectList.Add(new DoubanMovie()
                    {
                        Id = regexSubjetId.Match(titleNode.Attributes["href"].Value).Groups[1].Value,
                        AuthorName = "",
                        Author = new List<DoubanAuthor>() { new DoubanAuthor() { Name = "" } },
                        Summary = desc,
                        Title = title,
                        Image = imgNode.Attributes["src"].Value,
                        AltTitle = meta
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
            IsolatedStorageHelper.SaveFile<List<DoubanMovie>>(Globals.MOVIE_ALLLIST_FILENAME, AllList);
            IsolatedStorageHelper.SaveFile<List<DoubanMovie>>(Globals.MOVIE_ACTIONLIST_FILENAME, ActionList);
            IsolatedStorageHelper.SaveFile<List<DoubanMovie>>(Globals.MOVIE_PLOTLIST_FILENAME, PlotList);
            IsolatedStorageHelper.SaveFile<List<DoubanMovie>>(Globals.MOVIE_COMEDYLIST_FILENAME, ComedyList);
            IsolatedStorageHelper.SaveFile<List<DoubanMovie>>(Globals.MOVIE_SUSPENSELIST_FILENAME, SuspenseList);
        }
    }
}
