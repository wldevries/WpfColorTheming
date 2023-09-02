using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfColorTheming;

public class RotateBrushBehavior : Behavior<FrameworkElement>
{
    public LinearGradientBrush Brush
    {
        get { return (LinearGradientBrush)GetValue(BrushProperty); }
        set { SetValue(BrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Brush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty BrushProperty =
        DependencyProperty.Register("Brush", typeof(LinearGradientBrush), typeof(RotateBrushBehavior), new PropertyMetadata(null, BrushChanged));

    private static void BrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((RotateBrushBehavior)d).UpdateUI();
    }

    protected override void OnAttached()
    {
        this.AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;
        this.UpdateUI();
    }

    protected override void OnDetaching()
    {
        this.AssociatedObject.SizeChanged -= AssociatedObject_SizeChanged;
    }

    private void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        this.UpdateUI();
    }

    bool updating = false;
    private void UpdateUI()
    {
        if (this.Brush != null && this.AssociatedObject != null && !updating)
        {
            updating  = true;
            double atan = Math.Atan2(this.AssociatedObject.ActualWidth, this.AssociatedObject.ActualHeight);
            var a = (atan / Math.PI * 180);
            this.Brush.RelativeTransform = new TransformGroup()
            {
                Children = new(new Transform[]
                {
                    new RotateTransform(-a, 0.5, 0.5),
                    new ScaleTransform(this.AssociatedObject.ActualWidth / this.AssociatedObject.ActualHeight, 1, 0.5, 0.5),
                }),
            };
            updating = false;
        }
    }
}
