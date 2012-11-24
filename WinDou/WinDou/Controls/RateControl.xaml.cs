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

namespace WinDou.Controls
{
    public partial class RateControl : UserControl
    {
        public static readonly DependencyProperty RatingProperty = DependencyProperty.Register("Rating", typeof(double), typeof(RateControl), new PropertyMetadata(0.0d));

        public RateControl()
        {
            InitializeComponent();
        }

        public double Rating
        {
            get
            {
                return (double)GetValue(RatingProperty);
            }
            set
            {
                SetValue(RatingProperty, value);
            }
        }
    }
}
