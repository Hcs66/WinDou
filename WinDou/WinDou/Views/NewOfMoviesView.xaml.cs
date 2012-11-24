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
using Coding4Fun.Phone.Controls;

namespace WinDou.Views
{
    public partial class NewOfMoviesView : NewOfSubjectViewBase
    {
        public NewOfMoviesView()
        {
            InitializeComponent();
            DataContext=base.SubjectViewModel= App.NewOfMoviesViewModel;
            base.SubjectType = "1";
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