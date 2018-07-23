using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UniversalFormsToolkit.Prism.Controls
{
    public sealed partial class CustomContentDialog : ContentDialog
    {
        public CustomContentDialog()
        {
            this.InitializeComponent();
            this.Loaded += CustomContentDialog_Loaded;
        }

        private void CustomContentDialog_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var collection = VisualTreeHelper.GetOpenPopups(Window.Current);
            foreach (var item in collection)
            {
                if(item.Child is Windows.UI.Xaml.Shapes.Rectangle)
                {
                    var rectangle = item.Child as Windows.UI.Xaml.Shapes.Rectangle;
                    rectangle.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(60,7,7,7));
                }
            }
        }
    }
}
