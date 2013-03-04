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
using DoubanSharp.Model;
using WinDou.ViewModels;
using Coding4Fun.Toolkit.Controls;
using System.Collections.Generic;
using WinDou.Controls;

namespace WinDou.Views
{
    public class NewOfSubjectViewBase : WinDouAppPage
    {
        protected string SubjectType { get; set; }
        protected INewOfSubjectViewModelBase SubjectViewModel { get; set; }
        protected List<ProgressLLS> ProgressLLSList { get; set; }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                foreach (var list in ProgressLLSList)
                {
                    list.IsBusy = true;
                }
                SubjectViewModel.RegisteLoadCompleted(LoadCompleted);
                SubjectViewModel.LoadData();
            }
        }

        protected void LoadCompleted(object s, DoubanSearchCompletedEventArgs e)
        {
            foreach (var list in ProgressLLSList)
            {
                list.IsBusy = false;
            }
            SubjectViewModel.UnRegisteLoadCompleted(LoadCompleted);
            if (!e.IsSuccess)
            {
                ToastPrompt toast = new ToastPrompt();
                toast.Message = "加载信息出错，请重试";
                toast.Show();
            }
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
            foreach (var list in ProgressLLSList)
            {
                list.IsBusy = true;
            }
            SubjectViewModel.RegisteLoadCompleted(LoadCompleted);
            SubjectViewModel.LoadData(true);
        }

    }
}
