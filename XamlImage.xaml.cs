using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.Windows.Shapes.Path;

namespace Flow.Menu.Controls
{
    /// <summary>
    /// Interaction logic for XamlImage.xaml
    /// </summary>
    public partial class XamlImage : ContentControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source",
            typeof(Uri),
            typeof(XamlImage),
            new PropertyMetadata(default(Uri), OnSourceChanged));

        public static readonly DependencyProperty BrushProperty = DependencyProperty.Register(
            "Brush", typeof(Brush),
            typeof(XamlImage),
            new PropertyMetadata(new SolidColorBrush(Colors.White), null));

        public XamlImage()
        {
            this.InitializeComponent();
        }

        public Uri Source
        {
            get { return (Uri)this.GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }

        public Brush Brush
        {
            get { return (Brush)this.GetValue(BrushProperty); }
            set { this.SetValue(BrushProperty, value); }
        }

        private static void OnSourceChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var xamlImage = (XamlImage)dependencyObject;
            var value = (Uri)dependencyPropertyChangedEventArgs.NewValue;

            if (value == null)
                xamlImage.Content = null;
            else
                xamlImage.Content = xamlImage.OpenUri(value);
        }

        private object OpenUri(Uri uri)
        {
            try
            {
                Uri absolute;
                if (uri.IsAbsoluteUri)
                    absolute = new Uri(uri.AbsoluteUri);
                else
                    absolute = new Uri(BaseUriHelper.GetBaseUri(this), uri);

                switch (absolute.Scheme)
                {
                    case "file":
                        var file = (Viewbox)XamlReader.Load(OpenFileResource(absolute));
                        ParseVectorXaml(file);
                        return file;

                    case "pack":
                        var pack = (Viewbox)XamlReader.Load(OpenEmbeddedResource(absolute));
                        ParseVectorXaml(pack);
                        return pack;

                    // TODO: add more schemes when needed, for example http://
                    default:
                        throw new NotSupportedException($"Unsupported Uri format: {absolute.Scheme}");
                }
            }
            catch (IOException)
            {
                // Icon resource could not be found!
                // Return an ugly red square instead of the icon
                return new Rectangle
                {
                    Stretch = Stretch.Fill,
                    Fill = new SolidColorBrush(Colors.Red)
                };
            }
        }

        private static Stream OpenEmbeddedResource(Uri uri)
        {
            return Application.GetResourceStream(uri)?.Stream;
        }

        private static Stream OpenFileResource(Uri uri)
        {
            var file = new FileInfo(uri.AbsolutePath);
            return file.OpenRead();
        }

        private static void ParseVectorXaml(Viewbox viewbox)
        {
            // Illustrator automatically sets a width and height for the viewbox.
            // Change the width and height to NaN, so the image scales to its parent control.
            viewbox.Width = double.NaN;
            viewbox.Height = double.NaN;
            if (viewbox.Child is Canvas)
                ParseCanvas((Canvas)viewbox.Child);
        }

        // Illustrator makes a canvas for each layer and fills it with Path controls.
        // To set the color for each Path with a style, we need to reset the color provided by Illustrator.
        // Then repeat the process for each canvas within another canvas, for when we have layers within layers.
        private static void ParseCanvas(Canvas canvas)
        {
            for (var i = 0; i < canvas.Children.Count; i++)
            {
                var child = canvas.Children[i];
                if (child is Path)
                    child.ClearValue(Shape.FillProperty);
                if (child is Canvas)
                    ParseCanvas((Canvas)child);
            }
        }
    }
}