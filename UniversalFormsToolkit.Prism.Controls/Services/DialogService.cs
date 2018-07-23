using Prism.Windows.Navigation;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using UniversalFormsToolkit.Prism.Controls.Events;
using UniversalFormsToolkit.Prism.Controls.Interfaces;
using UniversalFormsToolkit.Prism.Controls.Models;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace UniversalFormsToolkit.Prism.Controls.Services
{
    public class DialogService : IDialogService
    {
        CustomContentDialog contentDialog = null;

        private static bool result;
        public bool Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
            }
        }

        public DialogService()
        {
            //  appService = IoC.Get<IAppService>();
            Window.Current.CoreWindow.SizeChanged += CoreWindow_SizeChanged;
        }

        private void CoreWindow_SizeChanged(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.WindowSizeChangedEventArgs args)
        {
            if (args.Size.IsEmpty)
                return;
          var popups=  VisualTreeHelper.GetOpenPopups(Window.Current);
            if (popups != null)
            {
              var modal=  popups.FirstOrDefault(x => x.Name == "ModalPopup");
                if (modal != null)
                {
                    var child = modal.Child as FrameworkElement;

            
                    modal.Width = args.Size.Width;
                    modal.Height = args.Size.Height;
                    child.Width = args.Size.Width;
                    child.Height = args.Size.Height;
                }
            }
        }

        public event EventHandler<DialogClosedEventArgs> DialogClosed;

        private static object DialogCloseParameter;


        private static bool? DialogResult { get; set; }
        public void Close(bool? dialogResult = null, object Parameter = null)
        {
            DialogCloseParameter = Parameter;
            DialogResult = dialogResult;
            if (Window.Current != null)
            {
                var control = Window.Current.Content as UserControl;
                if (control == null)
                {
                    return;
                }
                var grid = control.Content as Grid;

                if (grid == null)
                {
                    Debug.WriteLine("Control must have a Grid as main container");
                    return;
                }
                var popups = grid.Children.OfType<Popup>();
                if (popups != null && popups.Any())
                {
                    var modalPopup = popups.FirstOrDefault(x => x.Name == "ModalPopup");
                    if (modalPopup != null)
                    {
                        var grid2 = modalPopup.Child as Grid;
                        var border = grid2.Children.FirstOrDefault() as Border;
                        var rootContainer = border.Child as ContentControl;
                        if (rootContainer != null)
                        {
                            rootContainer.Content = null;
                        }
                        modalPopup.IsOpen = false;

                        grid.Children.Remove(modalPopup);
                    }
                }

            }

        }



        public async Task ShowAsync(FrameworkElement view, string title = "", object parameter = null, DialogParameters dialogParameters = null, bool AlwaysActiveViewOnOpen = true, bool AlwaysDeactiveViewOnClose = true)
        {
            var customDialog = new CustomDialog();
            if (dialogParameters == null)
                dialogParameters = new DialogParameters();

            FillParameters(customDialog, dialogParameters);


            if (view == null)
                throw new Exception("View not found");

            customDialog.Child = view;




            customDialog.Title = title;
            var modal = view.DataContext as IModalDialog;
            if (modal != null)
            {
               
                customDialog.Title = string.IsNullOrEmpty(title) ? modal.Title : title;
                if (modal.AcceptCommand != null)
                {




                    customDialog.AcceptCommandText = modal.AcceptLabel;
                    customDialog.AcceptCommand = modal.AcceptCommand;
                }



                if (modal.CancelCloseCommand != null)
                {

                    customDialog.CancelCommandText = modal.CancelLabel;

                    customDialog.CancelCommand = modal.CancelCloseCommand;

                }
            }
           
          
            modal = null;
            var activatable = view.DataContext as INavigationAware;
            if (activatable != null)
            {
                activatable.OnNavigatedTo(new NavigatedToEventArgs()
                {
                    NavigationMode = Windows.UI.Xaml.Navigation.NavigationMode.New,
                    Parameter= parameter
                },
                null
                );
            }


            var deactivator = view.DataContext as INavigationAware;
            if (deactivator != null)
            {

                if (Window.Current != null)
                {
                    var control = Window.Current.Content as UserControl;
                    if (control == null)
                    {
                        return;
                    }
                    var grid = control.Content as Grid;

                    if (grid == null)
                    {
                        Debug.WriteLine("Control must have a Grid as main container");
                        return;
                    }
                    var popups = grid.Children.OfType<Popup>();
                    if (popups != null && popups.Any())
                    {
                        var modalPopup = popups.FirstOrDefault(x => x.Name == "ModalPopup");
                    }
                    else
                    {
                        var modalPopup = createModalPopup(customDialog, grid,dialogParameters);


                        grid.Children.Add(modalPopup);

                        modalPopup.IsOpen = true;
                        var closedEvent = Observable.FromEventPattern(modalPopup, "Closed");

                        IDisposable subscriber = null;
                        subscriber = closedEvent.Subscribe(x =>
                        {
                            var container = customDialog.FindName("Container") as ContentControl;
                            if (container != null)
                                container.Content = null;

                            deactivator.OnNavigatingFrom(new NavigatingFromEventArgs()
                            {
                                NavigationMode = Windows.UI.Xaml.Navigation.NavigationMode.Back,

                            }, null,false);

                            DialogClosed?.Invoke(this, new DialogClosedEventArgs(Result, DialogCloseParameter));


                            deactivator = null;
                          


                            container = null;
                            customDialog = null;
                            if (view != null)
                            {
                                ((FrameworkElement)view).DataContext = null;
                                view = null;
                            }
                            GC.Collect();




                            subscriber.Dispose();
                            closedEvent = null;

                        });
                    }
                }
            }


        }

     

   
        private Popup createModalPopup(FrameworkElement element, FrameworkElement target,DialogParameters dialogParameters)
        {
            var popup = new Popup();

            popup.Name = "ModalPopup";


            popup.VerticalAlignment = VerticalAlignment.Stretch;
            popup.HorizontalAlignment = HorizontalAlignment.Stretch;
            popup.Width = target.ActualWidth;
            popup.Height = target.ActualHeight;
            //  popup.IsOpen = false;
            var grid = new Grid();
            grid.Background = dialogParameters.DialogScreenLayerBackground;
             
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            Binding minWBinding = new Binding();
            minWBinding.Path = new PropertyPath("ActualWidth");
            minWBinding.Mode = BindingMode.TwoWay;
            minWBinding.Source = target;
            grid.SetBinding(FrameworkElement.WidthProperty, minWBinding);


            Binding minHBinding = new Binding();
            minHBinding.Path = new PropertyPath("ActualHeight");
            minHBinding.Mode = BindingMode.TwoWay;
            minHBinding.Source = target;
            grid.SetBinding(FrameworkElement.HeightProperty, minHBinding);
            var border = new Border();
            border.MinWidth = 400;
            border.MinHeight = 400;
            border.Margin = new Thickness(0, 48, 0, 0);
            border.HorizontalAlignment = HorizontalAlignment.Center;
            border.VerticalAlignment = VerticalAlignment.Center;

            var contentControl = new ContentControl();
            contentControl.Name = "RootContainer";
            contentControl.HorizontalAlignment = HorizontalAlignment.Stretch;
            contentControl.VerticalAlignment = VerticalAlignment.Stretch;
            contentControl.VerticalContentAlignment = VerticalAlignment.Stretch;
            contentControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            contentControl.Transitions = new Windows.UI.Xaml.Media.Animation.TransitionCollection()
            {
                new EntranceThemeTransition()
                {
                     FromVerticalOffset=220
                }
            };
            contentControl.Content = element;
            border.Child = contentControl;
            grid.Children.Add(border);

            popup.Child = grid;
            return popup;
        }



        private void FillParameters(CustomDialog customDialog, DialogParameters dialogParameters)
        {
            customDialog.Background = dialogParameters.DialogBackground;
            customDialog.IsAcceptCommandEnabled = dialogParameters.IsAcceptCommandEnabled;
            customDialog.AcceptCommandIcon = dialogParameters.AcceptCommandIcon;
            customDialog.AcceptCommandText = dialogParameters.AcceptCommandText;
            customDialog.IsVisibleAcceptCommand = dialogParameters.IsAcceptCommandVisible;

            customDialog.IsCancelCommandEnabled = dialogParameters.IsCancelCommandEnabled;
            customDialog.CancelCommandIcon = dialogParameters.CancelCommandIcon;
            customDialog.CancelCommandText = dialogParameters.CancelCommandText;
            customDialog.IsVisibleCancelCommand = dialogParameters.IsCancelCommandVisible;

            customDialog.ShowAllCommandButtons = dialogParameters.ShowAllButtons;

            customDialog.AcceptButtonBackground = dialogParameters.AcceptButtonBackground;
            customDialog.CancelButtonBackground = dialogParameters.CancelButtonBackground;
            customDialog.AcceptButtonForeground = dialogParameters.AcceptButtonForeground;
            customDialog.CancelButtonForeground = dialogParameters.CancelButtonForeground;
        }

       


        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {


            sender.PrimaryButtonCommand?.Execute(null);
            args.Cancel = true;
        }





     
        private void ContentDialog_PrimaryButtonClick1(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            sender?.Hide();
        }



        public async Task<bool> AskMessageDialogAsync(string message, string title = "")
        {
            bool result = false;
            var dg = new MessageDialog(message, title);
            dg.Commands.Add(new UICommand("Aceptar", (res) =>
            {
                result = true;
            }));
            dg.Commands.Add(new UICommand("Cancelar", (res) =>
            {
                result = false;
            }));
            await dg.ShowAsync();
            return result;

        }

        public async Task<string> ShowInputDialogAsync(string body, string title)
        {
            contentDialog = new CustomContentDialog();
            contentDialog.MaxWidth = 600;

            contentDialog.Background = new SolidColorBrush(Colors.White);
            contentDialog.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            contentDialog.VerticalContentAlignment = VerticalAlignment.Stretch;
            contentDialog.IsSecondaryButtonEnabled = false;
            contentDialog.SecondaryButtonText = "Cancelar";
            contentDialog.PrimaryButtonText = "Aceptar";
            var stackPanel = new StackPanel();
            var txtBody = new TextBlock();
            txtBody.Text = body;
            var txt = new TextBox();
            txt.MinHeight = 250;
            txt.AcceptsReturn = true;

            stackPanel.Children.Add(txtBody);
            stackPanel.Children.Add(txt);
            contentDialog.Title = title;
            contentDialog.Content = stackPanel;
            await contentDialog.ShowAsync();
            var result = txt.Text;
            contentDialog = null;
            return result;
        }



    }
}
