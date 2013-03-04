using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinDou.ViewModels
{
    public interface INewOfSubjectViewModelBase
    {
        void LoadData();

        void LoadData(bool isRefresh);

        void RegisteLoadCompleted(Action<object, DoubanSearchCompletedEventArgs> LoadCompleted);

        void UnRegisteLoadCompleted(Action<object, DoubanSearchCompletedEventArgs> LoadCompleted);
    }
}
