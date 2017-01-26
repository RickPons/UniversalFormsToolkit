using Windows.UI.Xaml.Controls;
namespace AutoGenerateForm.Uwp.Controls
{
    internal class FieldContainerControl:ContentControl
    {

    
        public FieldContainerControl()
        {
          
            HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
            VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch;
            HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
            VerticalContentAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch;
            Stack = new StackPanel();
            this.Content = Stack;
        }

      

        public StackPanel Stack { get; set; }

    }
}
