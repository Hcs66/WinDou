using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Coding4Fun.Toolkit.Controls;
using DoubanSharp.Model;
using WinDou.ViewModels;

namespace WinDou.Views
{
    public partial class NewOfBooksView : NewOfSubjectViewBase
    {
        public NewOfBooksView()
        {
            InitializeComponent();
            DataContext = base.SubjectViewModel = App.NewOfBooksViewModel;
            base.SubjectType = "0";
            ProgressLLSList = new List<Controls.ProgressLLS>();
            ProgressLLSList.Add(listFiction);
            ProgressLLSList.Add(listLiterature);
        }

        protected void linkBtnViewSubject_Click(object sender, RoutedEventArgs e)
        {
            ViewSubject((RoundButton)sender);
        }

        protected void appBarBtnRefreshList_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void appbarMenuHome_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}