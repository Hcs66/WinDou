using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;


namespace WinDou.Controls
{
    [TemplatePart(Name = "PART_ProgressOverlay", Type = typeof(ProgressOverlay))]
    public class ProgressLLS : LongListSelector
    {
        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        // Using a DependencyProperty as the backing store for IsBusy.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(ProgressLLS), new PropertyMetadata(false, OnIsBusyChanged));

        private static void OnIsBusyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ProgressLLS control = (ProgressLLS)obj;
            //应用模板后才能引用part
            control.ApplyTemplate();
            ProgressOverlay progressOverlay = control.GetTemplateChild("PART_ProgressOverlay") as ProgressOverlay;         
            if (control.IsBusy)
            {
                control.IsEnabled = false;
                control.Opacity = 0.5;
                if (progressOverlay != null)
                {
                    progressOverlay.Show();
                }
            }
            else
            {
                control.IsEnabled = true;
                control.Opacity = 1;
                if (progressOverlay != null)
                {
                    progressOverlay.Hide();
                }
            }
        }


    }
}
