using System.Windows.Input;

namespace UniversalFormsToolkit.Prism.Controls.Interfaces
{
    public interface IModalDialog 
    {
        ICommand AcceptCommand { get; set; }

        ICommand CancelCloseCommand { get; set; }

        string Title { get; set; }

        string AcceptLabel { get; set; }
        string CancelLabel { get; set; }

        bool IsAcceptCommandEnabled { get; set; }

        bool IsCancelCloseCommandEnabled { get; set; }

    }
}
