using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfColorTheming
{
    public static class Brushers
    {




        public static Brush GetHoverBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(HoverBrushProperty);
        }

        public static void SetHoverBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(HoverBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for HoverBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HoverBrushProperty =
            DependencyProperty.RegisterAttached("HoverBrush", typeof(Brush), typeof(Brushers), new PropertyMetadata(null));




        public static Brush GetActiveBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(ActiveBrushProperty);
        }

        public static void SetActiveBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(ActiveBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for ActiveBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveBrushProperty =
            DependencyProperty.RegisterAttached("ActiveBrush", typeof(Brush), typeof(Brushers), new PropertyMetadata(null));


    }
}
