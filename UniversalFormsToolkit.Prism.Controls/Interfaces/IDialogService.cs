using System;
using System.Threading.Tasks;
using UniversalFormsToolkit.Prism.Controls.Models;
using Windows.UI.Xaml;

namespace UniversalFormsToolkit.Prism.Controls.Interfaces
{
    public interface IDialogService
    {
        Task ShowAsync(FrameworkElement view, string title = "", object parameter = null, DialogParameters dialogParameters = null, bool AlwaysActiveViewOnOpen = true, bool AlwaysDeactiveViewOnClose = true);
        void Close(bool? dialogResult = null, object Parameter = null);
        Task<bool> AskMessageDialogAsync(string message, string title = "");
        Task<string> ShowInputDialogAsync(string body, string title);
        event EventHandler<Events.DialogClosedEventArgs> DialogClosed;

        bool Result { get; set; }


    }
}
