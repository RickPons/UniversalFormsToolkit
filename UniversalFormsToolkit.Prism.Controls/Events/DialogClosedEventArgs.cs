using System;

namespace UniversalFormsToolkit.Prism.Controls.Events
{
    public class DialogClosedEventArgs : EventArgs
    {
        public bool DialogResult { get; set; }
        public object Parameter { get; set; }

        public DialogClosedEventArgs(bool? dialogResult = false, object parameter = null)
        {
            if (dialogResult != null)
            {
                DialogResult = (bool)dialogResult;
            }
            else
            {
                DialogResult = false;
            }

            Parameter = parameter;
        }
    }
}
