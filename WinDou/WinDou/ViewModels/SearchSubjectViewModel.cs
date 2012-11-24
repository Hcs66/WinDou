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
using System.Collections.ObjectModel;
using DoubanSharp.Model;
using System.Collections.Generic;
using System.ComponentModel;
using HcsLib.WindowsPhone.ViewModel;

namespace WinDou.ViewModels
{
    public class SearchSubjectViewModel : ViewModelBase
    {
        private int rowPerPages = 10;
        private int currentSearchPageIndex = 1;
        public SearchSubjectViewModel()
        {
            SearchSubjectList = new ObservableCollection<BaseSubjectViewModel>();
            this.PropertyChanged += new PropertyChangedEventHandler(SearchSubjectViewModel_PropertyChanged);
        }

        void SearchSubjectViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
            switch (propertyName)
            {
                case "SearchSubjectList":
                    if (SearchCompleted != null)
                    {
                        SearchCompleted(null, new DoubanSearchCompletedEventArgs() {  IsSuccess=true});
                    }
                    break;
                default:
                    break;
            }
        }

        public override void LoadData()
        {

        }

        #region 属性

        /// <summary>
        /// 搜索结束事件
        /// </summary>
        public event EventHandler<DoubanSearchCompletedEventArgs> SearchCompleted;

        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 搜索类型
        /// </summary>
        public int SearchType { get; set; }

        /// <summary>
        /// 结果集
        /// </summary>
        public ObservableCollection<BaseSubjectViewModel> SearchSubjectList { get; set; }

        /// <summary>
        /// 加载更多数据按钮可见性
        /// </summary>
        public Visibility LoadMoreVisibility { get; set; }

        #endregion

        #region 方法

        public void SearchSubject(bool isPaging = false)
        {
            this.IsBusy = true;
            if (isPaging)
            {
                currentSearchPageIndex += rowPerPages;
            }
            else
            {
                currentSearchPageIndex = 1;
                this.SearchSubjectList = new ObservableCollection<BaseSubjectViewModel>();
            }
            switch (SearchType)
            {
                case 0://书籍搜索
                    App.DoubanService.SearchBooks(KeyWord, "", currentSearchPageIndex.ToString(), rowPerPages.ToString(),
                        (result, resp) =>
                        {
                            List<BookViewModel> vmList = new List<BookViewModel>();
                            if (resp.StatusCode == HttpStatusCode.OK && result.EntryList != null && result.EntryList.Count > 0)
                            {
                                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    foreach (var item in result.EntryList)
                                    {
                                        this.SearchSubjectList.Add(new BookViewModel(item));
                                    }
                                    this.LoadMoreVisibility = this.SearchSubjectList.Count < result.TotalResults ? Visibility.Visible : Visibility.Collapsed;
                                    this.OnPropertyChanged("LoadMoreVisibility");
                                    this.OnPropertyChanged("SearchSubjectList");
                                    this.IsBusy = false;
                                });
                            }
                            else if (SearchCompleted != null)
                            {
                                this.IsBusy = false;
                                SearchCompleted(null, new DoubanSearchCompletedEventArgs() {  IsSuccess=false});
                            }
                        }
                    );
                    break;
                case 1://电影搜索
                    App.DoubanService.SearchMovies(KeyWord, "", currentSearchPageIndex.ToString(), rowPerPages.ToString(),
                        (result, resp) =>
                        {
                            List<MovieViewModel> vmList = new List<MovieViewModel>();
                            if (resp.StatusCode == HttpStatusCode.OK && result.EntryList != null && result.EntryList.Count > 0)
                            {
                                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    foreach (var item in result.EntryList)
                                    {
                                        this.SearchSubjectList.Add(new MovieViewModel(item));
                                    }
                                    this.LoadMoreVisibility = this.SearchSubjectList.Count < result.TotalResults ? Visibility.Visible : Visibility.Collapsed;
                                    this.OnPropertyChanged("LoadMoreVisibility");
                                    this.OnPropertyChanged("SearchSubjectList");
                                    this.IsBusy = false;
                                });
                            }
                            else if (SearchCompleted != null)
                            {
                                SearchCompleted(null, new DoubanSearchCompletedEventArgs() { IsSuccess = false });
                            }
                        }
                    );
                    break;
                case 2://音乐搜索
                    App.DoubanService.SearchMusic(KeyWord, "", currentSearchPageIndex.ToString(), rowPerPages.ToString(),
                        (result, resp) =>
                        {
                            List<MusicViewModel> vmList = new List<MusicViewModel>();
                            if (resp.StatusCode == HttpStatusCode.OK && result.EntryList != null && result.EntryList.Count > 0)
                            {
                                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    foreach (var item in result.EntryList)
                                    {
                                        this.SearchSubjectList.Add(new MusicViewModel(item));
                                    }
                                    this.LoadMoreVisibility = this.SearchSubjectList.Count < result.TotalResults ? Visibility.Visible : Visibility.Collapsed;
                                    this.OnPropertyChanged("LoadMoreVisibility");
                                    this.OnPropertyChanged("SearchSubjectList");
                                    this.IsBusy = false;
                                });
                            }
                            else if (SearchCompleted != null)
                            {
                                SearchCompleted(null, new DoubanSearchCompletedEventArgs() { IsSuccess = false });
                            }
                        }
                    );
                    break;
                default:
                    break;
            }
        }

        #endregion


    }
}
