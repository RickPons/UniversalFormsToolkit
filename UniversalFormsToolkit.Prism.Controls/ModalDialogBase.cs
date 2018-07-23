using Prism.Mvvm;
using System;
using System.Windows.Input;
using UniversalFormsToolkit.Prism.Controls.Interfaces;

namespace UniversalFormsToolkit.Prism.Controls
{
    public abstract class ModalDialogBase : BindableBase, IModalDialog
    {
        readonly IDialogService dialogService;
        public ModalDialogBase()
        {
            //dialogService = IoC.Get<IDialogService>();
            this.AcceptLabel = "Aceptar";
            this.CancelLabel = "Cancelar";
            //this.AcceptCommand = new RelayCommand(AcceptCommandExecuteAsync, CanAcceptCommandExecute);
            //this.AcceptCommand.CanExecuteChanged += AcceptCommand_CanExecuteChanged;
            //this.CancelCloseCommand = new RelayCommand(CancelCloseCommandExecute);
            this.IsCancelCloseCommandEnabled = true;
        }

        public void ForceToCloseDialog()
        {
            dialogService.Result = true;
            dialogService.Close();
        }
        public virtual bool CanAcceptCommandExecute()
        {
            IsAcceptCommandEnabled = true;
            return IsAcceptCommandEnabled;
        }

        private void AcceptCommand_CanExecuteChanged(object sender, EventArgs e)
        {
            AcceptCommand?.CanExecute(IsAcceptCommandEnabled);
        }

        protected virtual void CancelCloseCommandExecute()
        {
            dialogService.Result = false;
        }

        protected virtual void AcceptCommandExecuteAsync()
        {
            dialogService.Result = true;
            dialogService.Close();
        }
        public ICommand AcceptCommand { get; set; }


        public string AcceptLabel { get; set; }

        public ICommand CancelCloseCommand { get; set; }


        public string CancelLabel { get; set; }

        private string title;
        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
                RaisePropertyChanged();
            }
        }

        public object Parameter { get; set; }

        private bool isAcceptCommandEnabled;
        public bool IsAcceptCommandEnabled
        {
            get
            {
                return isAcceptCommandEnabled;
            }

            set
            {
                isAcceptCommandEnabled = value;
                RaisePropertyChanged();
            }
        }


        private bool isCancelCloseCommandEnabled;
        public bool IsCancelCloseCommandEnabled
        {
            get
            {
                return isCancelCloseCommandEnabled;
            }

            set
            {
                isCancelCloseCommandEnabled = value;
                RaisePropertyChanged();
            }
        }

    }
}
