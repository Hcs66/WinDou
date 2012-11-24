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
using Coding4Fun.Phone.Controls;
using WinDou.ViewModels;

namespace WinDou.Views
{
    public class NewOfSubjectViewBase : WinDouAppPage
    {
        protected string SubjectType { get; set; }
        protected NewOfSubjectViewModelBase SubjectViewModel { get; set; }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            SubjectViewModel.LoadCompleted += LoadCompleted;
            SubjectViewModel.LoadData();
            base.OnNavigatedTo(e);
        }

        protected void LoadCompleted(object s, EventArgs e)
        {
            base.SetProgressIndicator(false);
            SubjectViewModel.LoadCompleted -= LoadCompleted;
        }

        protected void ViewSubject(RoundButton button)
        {
            if (button.CommandParameter != null && !string.IsNullOrEmpty(button.CommandParameter.ToString()))
            {
                string url = "/Views/Subject/SubjectView.xaml?subjectId=" + button.CommandParameter.ToString() + "&type=" + SubjectType;
                NavigationService.Navigate(new Uri(url, UriKind.Relative));
            }
        }

        protected void RefreshList()
        {
            base.SetProgressIndicator(true);
            SubjectViewModel.LoadCompleted += LoadCompleted;
            SubjectViewModel.LoadData(true);
        }

    }
}
