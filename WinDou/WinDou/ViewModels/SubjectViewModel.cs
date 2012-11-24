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
using System.Linq;
using DoubanSharp.Model;
using DoubanSharp.Model.Enum;
using System.Collections.Generic;
using System.Text;
using HcsLib.WindowsPhone.ViewModel;
using HcsLib.WindowsPhone.Extensions;

namespace WinDou.ViewModels
{
    public class SubjectViewModel : ViewModelBase
    {

        public SubjectViewModel()
        {

        }

        #region 属性

        public event EventHandler<DoubanSearchCompletedEventArgs> GetSubjectCompleted;

        #endregion

        #region 方法

        public void GetSubject(string subjectId, string type)
        {
            switch (type)
            {
                case "0":
                    App.DoubanService.GetBook(subjectId,
                          (subject, resp) =>
                          {
                              BaseSubjectViewModel subjectViewModel = null;
                              if (resp.StatusCode == HttpStatusCode.OK && subject != null)
                              {
                                  subjectViewModel = new BookViewModel(subject);
                              }
                              if (GetSubjectCompleted != null)
                              {
                                  GetSubjectCompleted(null, new DoubanSearchCompletedEventArgs() { IsSuccess = resp.StatusCode == HttpStatusCode.OK, Message = "", Result = subjectViewModel });
                              }
                          }
                      );
                    break;
                case "1":
                    App.DoubanService.GetMovie(subjectId,
                          (subject, resp) =>
                          {
                              BaseSubjectViewModel subjectViewModel = null;
                              if (resp.StatusCode == HttpStatusCode.OK && subject != null)
                              {
                                  subjectViewModel = new MovieViewModel(subject);
                              }
                              if (GetSubjectCompleted != null)
                              {
                                  GetSubjectCompleted(null, new DoubanSearchCompletedEventArgs() { IsSuccess = resp.StatusCode == HttpStatusCode.OK, Message = "", Result = subjectViewModel });
                              }
                          }
                      );
                    break;
                case "2":
                    App.DoubanService.GetMusic(subjectId,
                          (subject, resp) =>
                          {
                              BaseSubjectViewModel subjectViewModel = null;
                              if (resp.StatusCode == HttpStatusCode.OK && subject != null)
                              {
                                  subjectViewModel = new MusicViewModel(subject);
                              }
                              if (GetSubjectCompleted != null)
                              {
                                  GetSubjectCompleted(null, new DoubanSearchCompletedEventArgs() { IsSuccess = resp.StatusCode == HttpStatusCode.OK, Message = "", Result = subjectViewModel });
                              }
                          }
                      );
                    break;
                default:
                    break;
            }

        }

        #endregion

        public override void LoadData()
        {

        }
    }

    public class BaseSubjectViewModel : ViewModelBase
    {
        protected BaseSubjectViewModel(DoubanSubject subject)
        {
            m_DoubanSubject = subject;
            SubjectId = subject.Id.Substring(subject.Id.TrimEnd('/').LastIndexOf("/") + 1).TrimEnd('/');
            Title = m_DoubanSubject.Title;
            if (m_DoubanSubject.Author != null)
            {
                AuthorName = m_DoubanSubject.Author.AuthorName;
            }
            SubjectImage = m_DoubanSubject.Links.FirstOrDefault(l => l.Rel == "image").Href;
            Summary = m_DoubanSubject.Summary;
            Kind = subject.Kind;
            Rating = subject.Rating;
        }

        #region 属性

        protected DoubanSubject m_DoubanSubject;
        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; this.OnPropertyChanged("Title"); }
        }
        private string authorName;
        public string AuthorName
        {
            get { return authorName; }
            set { authorName = value; this.OnPropertyChanged("AuthorName"); }
        }
        private string subjectImage;
        public string SubjectImage
        {
            get { return subjectImage; }
            set { subjectImage = value; this.OnPropertyChanged("SubjectImage"); }
        }
        private string summary;
        public string Summary
        {
            get { return summary; }
            set { summary = value; this.OnPropertyChanged("Summary"); }
        }
        public SubjectKind Kind { get; set; }
        public DoubanRating Rating { get; set; }
        public string SubjectId { get; set; }

        #endregion

        #region 方法


        public override void LoadData()
        {

        }

        protected string GetAttributeValue(string name)
        {
            if (m_DoubanSubject.Attributes.IsExists(a => a.Name == name))
            {
                return m_DoubanSubject.Attributes.FirstOrDefault(a => a.Name == name).Value;
            }
            return "";
        }

        protected string GetAttributeValues(string name)
        {
            if (m_DoubanSubject.Attributes.IsExists(a => a.Name == name))
            {
                StringBuilder values = new StringBuilder();
                m_DoubanSubject.Attributes.Where(a => a.Name == name).ToList().ForEach(a => values.AppendString(a.Value, "/"));
                return values.ToString().TrimEnd('/');
            }
            return "";
        }
        #endregion
    }

    public class BookViewModel : BaseSubjectViewModel
    {

        public BookViewModel(DoubanSubject subject)
            : base(subject)
        {
            if (m_DoubanSubject.Attributes != null && m_DoubanSubject.Attributes.Count > 0)
            {
                Publisher = GetAttributeValue("publisger");
                Pubdate = GetAttributeValue("pubdate");
                Price = GetAttributeValue("price");
                Binding = GetAttributeValue("binding");
                ISBN = GetAttributeValue("isbn13");
                AuthorIntro = GetAttributeValue("author-intro");
                Pages = GetAttributeValue("pages");
            }
        }
        public string Publisher { get; set; }
        public string Pubdate { get; set; }
        public string Price { get; set; }
        public string Binding { get; set; }
        public string ISBN { get; set; }
        public string AuthorIntro { get; set; }
        public string Pages { get; set; }
    }

    public class MovieViewModel : BaseSubjectViewModel
    {
        public MovieViewModel(DoubanSubject subject)
            : base(subject)
        {
            if (m_DoubanSubject.Attributes != null && m_DoubanSubject.Attributes.Count > 0)
            {
                Year = GetAttributeValue("year");
                Pubdate = GetAttributeValue("pubdate");
                Directors = GetAttributeValues("director");
                Languages = GetAttributeValues("lanuage");
                Casts = GetAttributeValues("cast");
                MovieTypes = GetAttributeValues("movie_type");
                Writers = GetAttributeValues("writer");
                AKAs = GetAttributeValues("aka");
                Site = GetAttributeValue("site");
                IMDB = GetAttributeValue("imdb");
                Country = GetAttributeValue("country");
                MovieDuration = GetAttributeValue("movie_duration");
                Episodes = GetAttributeValue("episodes");
            }
        }
        public string Year { get; set; }
        public string Directors { get; set; }
        public string Languages { get; set; }
        public string Site { get; set; }
        public string IMDB { get; set; }
        public string Country { get; set; }
        public string Casts { get; set; }
        public string MovieTypes { get; set; }
        public string Writers { get; set; }
        public string Pubdate { get; set; }
        public string AKAs { get; set; }
        public string MovieDuration { get; set; }
        public string Episodes { get; set; }
    }

    public class MusicViewModel : BaseSubjectViewModel
    {
        public MusicViewModel(DoubanSubject subject)
            : base(subject)
        {
            if (m_DoubanSubject.Attributes != null && m_DoubanSubject.Attributes.Count > 0)
            {
                Discs = GetAttributeValue("discs");
                Version = GetAttributeValue("version");
                EAN = GetAttributeValue("EAN");
                Tracks = GetAttributeValue("tracks");
                Pubdate = GetAttributeValue("pubdate");
                Singers = GetAttributeValues("singer");
                Publisher = GetAttributeValue("publisher");
                Media = GetAttributeValue("media");
            }
        }
        public string Discs { get; set; }
        public string Version { get; set; }
        public string EAN { get; set; }
        public string Tracks { get; set; }
        public string Pubdate { get; set; }
        public string Singers { get; set; }
        public string Publisher { get; set; }
        public string Media { get; set; }
    }
}
