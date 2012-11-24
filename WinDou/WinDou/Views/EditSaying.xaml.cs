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

namespace WinDou.Views
{
    public partial class EditSaying : EntityEditingPage
    {
        public EditSaying()
        {
            InitializeComponent();

            DataContext = App.EditSayingViewModel;
        }

        private void appBarBtnCancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void appBarBtnSave_Click(object sender, EventArgs e)
        {
            if (App.DoubanService.HasAuthenticated)
            {
                App.DoubanService.AddMiniBlog(new DoubanSharp.Model.DoubanMiniBlog() { Content = txtContent.Text },
                    resp =>
                    {
                        if (resp.StatusCode == HttpStatusCode.Created)
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(
                                () =>
                                {
                                    
                                    if (MessageBox.Show("发表成功！", "提示信息", MessageBoxButton.OK) == MessageBoxResult.OK)
                                    {
                                        NavigationService.GoBack();
                                    }
                                });
                        }
                    });
            }
        }
    }
}