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
            this.SubjectListUrl = "http://movie.douban.com/";
        }

        protected override void BuildSubjectList(HtmlNode root)
        {
            AllList = ParseSubjecList(root.SelectNodes("//ul[@id='hots-movie-all']//li"));
            ActionList = ParseSubjecList(root.SelectNodes("//ul[@id='hots-movie-action']//li"));
            PlotList = ParseSubjecList(root.SelectNodes("//ul[@id='hots-movie-drama']//li"));
            ComedyList = ParseSubjecList(root.SelectNodes("//ul[@id='hots-movie-comedy']//li"));
            SuspenseList = ParseSubjecList(root.SelectNodes("//ul[@id='hots-movie-mystery']//li"));
        }

        protected override List<DoubanMovie> ParseSubjecList(IEnumerable<HtmlAbstractor> itemList)
        {
            List<DoubanMovie> subjectList = new List<DoubanMovie>();
            foreach (var item in itemList)
            {
                HtmlNodeCollection movieNodes = (item as HtmlElementAbstractor).Element.ChildNodes;
                if (movieNodes.Count == 0)
                {
                    continue;
                }
                //标题和链接
                HtmlNode h3 = movieNodes.FindFirst("h3");
                HtmlNode titleNode = h3.LastChild;

                HtmlNode infoNode = movieNodes.SingleOrDefault(n => n.HasAttributes && n.Attributes.Contains("class")
                    && n.Attributes["class"].Value == "hots-tab-info");
                HtmlNodeCollection infoNodeChilds = infoNode.ChildNodes;
                string meta = "";
                string desc = "";
                if (infoNodeChilds != null && infoNodeChilds.Count > 0)
                {
                    //评分信息
                    var metaNode = infoNodeChilds.SingleOrDefault(n => n.HasAttributes && n.Attributes.Contains("class")
                        && n.Attributes["class"].Value == "hots-meta");
                    if (metaNode != null)
                    {
                        meta = metaNode.InnerText.Replace("\n", "").Replace(" ", "");
                    }
                    //描述信息
                    var descNode = infoNodeChilds.SingleOrDefault(n => n.HasAttributes && n.Attributes.Contains("class")
                        && n.Attributes["class"].Value == "hots-desc");
                    if (descNode != null)
                    {
                        desc = descNode.InnerText.Replace("\n", "").Replace(" ", "");
                    }
                }

                //图片
                HtmlNode img = movieNodes.FindFirst("img");
                subjectList.Add(new DoubanMovie()
                {
                    Id = regexSubjetId.Match(titleNode.Attributes["href"].Value).Groups[1].Value,
                    AuthorName = "",
                    Author = new List<DoubanAuthor>() { new DoubanAuthor() { Name = "" } },
                    Summary = desc,
                    Title = regexRemoveBlank.Replace(titleNode.InnerText, ""),
                    Image = img.Attributes["src"].Value,
                    AltTitle = meta
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
            IsolatedStorageHelper.SaveFile<List<DoubanMovie>>(Globals.MOVIE_ALLLIST_FILENAME, AllList);
            IsolatedStorageHelper.SaveFile<List<DoubanMovie>>(Globals.MOVIE_ACTIONLIST_FILENAME, ActionList);
            IsolatedStorageHelper.SaveFile<List<DoubanMovie>>(Globals.MOVIE_PLOTLIST_FILENAME, PlotList);
            IsolatedStorageHelper.SaveFile<List<DoubanMovie>>(Globals.MOVIE_COMEDYLIST_FILENAME, ComedyList);
            IsolatedStorageHelper.SaveFile<List<DoubanMovie>>(Globals.MOVIE_SUSPENSELIST_FILENAME, SuspenseList);
        }
    }
}
