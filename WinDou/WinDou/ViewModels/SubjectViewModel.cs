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
//using DoubanSharp.Model.Enum;
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

        public bool HaveComments { get; set; }

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
                              if (resp.RestResponse.StatusCode == HttpStatusCode.OK && subject != null)
                              {
                                  subjectViewModel = new BookViewModel(subject);
                              }
                              if (GetSubjectCompleted != null)
                              {
                                  GetSubjectCompleted(null, new DoubanSearchCompletedEventArgs() { IsSuccess = resp.RestResponse.StatusCode == HttpStatusCode.OK, Message = "", Result = subjectViewModel });
                              }
                          }
                      );
                    break;
                case "1":
                    App.DoubanService.GetMovie(subjectId,
                          (subject, resp) =>
                          {
                              BaseSubjectViewModel subjectViewModel = null;
                              if (resp.RestResponse.StatusCode == HttpStatusCode.OK && subject != null)
                              {
                                  subjectViewModel = new MovieViewModel(subject);
                              }
                              if (GetSubjectCompleted != null)
                              {
                                  GetSubjectCompleted(null, new DoubanSearchCompletedEventArgs() { IsSuccess = resp.RestResponse.StatusCode == HttpStatusCode.OK, Message = "", Result = subjectViewModel });
                              }
                          }
                      );
                    break;
                case "2":
                    App.DoubanService.GetMusic(subjectId,
                          (subject, resp) =>
                          {
                              BaseSubjectViewModel subjectViewModel = null;
                              if (resp.RestResponse.StatusCode == HttpStatusCode.OK && subject != null)
                              {
                                  subjectViewModel = new MusicViewModel(subject);
                              }
                              if (GetSubjectCompleted != null)
                              {
                                  GetSubjectCompleted(null, new DoubanSearchCompletedEventArgs() { IsSuccess = resp.RestResponse.StatusCode == HttpStatusCode.OK, Message = "", Result = subjectViewModel });
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
        protected BaseSubjectViewModel(DoubanSubjectBase subject)
        {
            m_DoubanSubject = subject;
            SubjectId = subject.DoubanObjectId;
            Title = m_DoubanSubject.Title;
            SubjectImage = m_DoubanSubject.Image;
            Summary = m_DoubanSubject.Summary;
            //Kind = subject.Kind;
            Rating = subject.Rating;
        }

        #region 属性

        protected DoubanSubjectBase m_DoubanSubject;
        public string Title { get; set; }
        public string SubjectImage { get; set; }
        public string Summary { get; set; }
        //public SubjectKind Kind { get; set; }
        public DoubanRating Rating { get; set; }
        public string SubjectId { get; set; }
        public string AuthorName { get; set; }

        #endregion

        #region 方法


        public override void LoadData()
        {

        }

        protected string GetAttributeValues(string name)
        {
            if (m_DoubanSubject.Attrs.ContainsKey(name))
            {
                return string.Join("/", m_DoubanSubject.Attrs[name]);
            }
            return "";
        }
        #endregion
    }

    public class BookViewModel : BaseSubjectViewModel
    {

        public BookViewModel(DoubanBook book)
            : base(book)
        {
            Publisher = book.Publisher;
            Pubdate = string.Format("{0:yyyy-MM-dd}", book.Pubdate);
            Price = book.Price;
            Binding = book.Binding;
            ISBN = book.Isbn13;
            AuthorIntro = book.AuthorIntro;
            Pages = book.Pages;
            AuthorName = string.Join("/", book.Author);
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
        public MovieViewModel(DoubanMovie movie)
            : base(movie)
        {
            if (m_DoubanSubject.Attrs != null && m_DoubanSubject.Attrs.Count > 0)
            {
                Year = GetAttributeValues("year");
                Pubdate = GetAttributeValues("pubdate");
                Directors = GetAttributeValues("director");
                Languages = GetAttributeValues("lanuage");
                Casts = GetAttributeValues("cast");
                MovieTypes = GetAttributeValues("movie_type");
                Writers = GetAttributeValues("writer");
                AKAs = GetAttributeValues("title");
                Country = GetAttributeValues("country");
                MovieDuration = GetAttributeValues("movie_duration");
                Episodes = GetAttributeValues("episodes");
            }
        }
        public string Year { get; set; }
        public string Directors { get; set; }
        public string Languages { get; set; }
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
        public MusicViewModel(DoubanMusic music)
            : base(music)
        {
            if (m_DoubanSubject.Attrs != null && m_DoubanSubject.Attrs.Count > 0)
            {
                Discs = GetAttributeValues("discs");
                Version = GetAttributeValues("version");
                EAN = GetAttributeValues("EAN");
                Tracks = GetAttributeValues("tracks");
                Pubdate = GetAttributeValues("pubdate");
                Singers = GetAttributeValues("singer");
                Publisher = GetAttributeValues("publisher");
                Media = GetAttributeValues("media");
            }
            var authors = from author in music.Author
                          select author.Name;
            AuthorName = string.Join("/", authors);
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
