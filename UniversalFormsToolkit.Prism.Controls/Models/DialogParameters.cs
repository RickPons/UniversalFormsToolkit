using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace UniversalFormsToolkit.Prism.Controls.Models
{
    public class DialogParameters
    {

        public DialogParameters()
        {
            
            DialogScreenLayerBackground = new SolidColorBrush(Color.FromArgb(60, 0, 0, 0));
            DialogBackground =(SolidColorBrush) Application.Current.Resources["ApplicationPageBackgroundThemeBrush"];
            AcceptButtonBackground= (SolidColorBrush)Application.Current.Resources["ButtonBackgroundThemeBrush"];
            CancelButtonBackground= (SolidColorBrush)Application.Current.Resources["ButtonBackgroundThemeBrush"];
            AcceptButtonForeground= (SolidColorBrush)Application.Current.Resources["ButtonForegroundThemeBrush"];
            CancelButtonForeground= (SolidColorBrush)Application.Current.Resources["ButtonForegroundThemeBrush"];
            AcceptCommandText = "Aceptar";
            IsAcceptCommandEnabled = true;
            IsAcceptCommandVisible = true;
            AcceptCommandIcon = "\xE8FB";

            CancelCommandIcon = "\xE711";
            CancelCommandText = "Cancelar";
            IsCancelCommandEnabled = true;
            IsCancelCommandVisible = true;
            ShowAllButtons = true;
        }

        public Brush AcceptButtonBackground { get; set; }
        public Brush CancelButtonBackground { get; set; }

        public Brush AcceptButtonForeground { get; set; }
        public Brush CancelButtonForeground { get; set; }
        public Brush DialogScreenLayerBackground { get; set; }
        public Brush DialogBackground { get; set; }
        public string AcceptCommandText { get; set; }
        public bool IsAcceptCommandEnabled { get; set; }
        public bool IsAcceptCommandVisible { get; set; }
        public string AcceptCommandIcon { get; set; }

        public string CancelCommandText { get; set; }
        public bool IsCancelCommandEnabled { get; set; }
        public bool IsCancelCommandVisible { get; set; }
        public string CancelCommandIcon { get; set; }

        public bool ShowAllButtons { get; set; }




    }
}
