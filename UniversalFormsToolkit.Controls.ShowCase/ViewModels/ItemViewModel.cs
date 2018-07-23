using Prism.Commands;
using Prism.Windows.Mvvm;
using System;
using System.Windows.Input;
using UniversalFormsToolkit.Prism.Controls.Interfaces;
using Windows.UI.Popups;

namespace UniversalFormsToolkit.Controls.ShowCase.ViewModels
{
    public class ItemViewModel: ViewModelBase, IModalDialog
    {


        IDialogService dialogService = null;

        #region Modal Dialog Properties
        public ICommand AcceptCommand { get; set; }
        public ICommand CancelCloseCommand { get; set; }
        public string Title { get; set; }
        public string AcceptLabel { get; set; }
        public string CancelLabel { get; set; }
        public bool IsAcceptCommandEnabled { get; set; }
        public bool IsCancelCloseCommandEnabled { get; set; }

        #endregion 


        public ItemViewModel( IDialogService _dialogService)
        {
            dialogService = _dialogService;
            AcceptCommand = new DelegateCommand(AcceptCommandExecute);
            CancelCloseCommand = new DelegateCommand(CancelCommandExecute);
            Title = "Create New Order";
            AcceptLabel = "Start";
            CancelLabel = "Cancel";
        }

    

        private void CancelCommandExecute()
        {
            dialogService.Close();
        }

        private async void AcceptCommandExecute()
        {
            await new MessageDialog("This is a Modal Dialog", "Universal Forms Toolkit Controls").ShowAsync();
            dialogService.Close();
        }
    }
}
