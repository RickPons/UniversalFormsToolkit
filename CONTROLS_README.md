# UniversalFormsToolkit Controls

This is a small set of controls to make easier for developers building UWP Applications using PRISM Framework

## Modal Dialog

There is a ContentDialog control for UWP but has some limitations. You can read about of this control here

https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.ContentDialog

If you are using MVVM Pattern with PRISM Framework may be you know this control is not friendly with MVVM Pattern.

UniversalFormsToolkit has a Service called DialogService with this service you can show your view as a Modal Dialog with just few lines of code.

```csharp
    var dialogService = ServiceLocator.Current.GetInstance<IDialogService>();
            var itemView = new ItemView();
            dialogService.ShowAsync(itemView, "Custom Titme", "Parameter");
```        
The viewmodel must implement the interface IModalDialog

```csharp
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
```   

You can see the full example downloading the code of this repository


# Windows Manager Service( In Progress)
