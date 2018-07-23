using System;
using Windows.UI.Xaml.Controls;

namespace UniversalFormsToolkit.Prism.Controls
{
    public class ManagedUserControl : UserControl
    {
        public ManagedUserControl()
        {
            this.Unloaded += ControlBase_Unloaded;
        }

        public virtual void ControlBase_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                if (DataContext != null)
                {
                    DataContext = null;

                    this.Unloaded -= ControlBase_Unloaded;
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
