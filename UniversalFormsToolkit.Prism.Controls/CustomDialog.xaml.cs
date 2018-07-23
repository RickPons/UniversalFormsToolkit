using Microsoft.Practices.ServiceLocation;
using System.Windows.Input;
using UniversalFormsToolkit.Prism.Controls.Interfaces;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace UniversalFormsToolkit.Prism.Controls
{
    public sealed partial class CustomDialog : ManagedUserControl
    {

        //IAppService appService = null;
        IDialogService dialogService = null;
        public CustomDialog()
        {
            this.InitializeComponent();
            if (!DesignMode.DesignModeEnabled)
            {
                dialogService = ServiceLocator.Current.GetInstance<IDialogService>();
            }


        }


        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CustomDialog), new PropertyMetadata("My title"));

        public ICommand AcceptCommand
        {
            get { return (ICommand) GetValue(AcceptCommandProperty); }
            set { SetValue(AcceptCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AcceptCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AcceptCommandProperty =
            DependencyProperty.Register("AcceptCommand", typeof(ICommand), typeof(CustomDialog), new PropertyMetadata(null));


        public ICommand CancelCommand
        {
            get { return (ICommand) GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CancelCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(CustomDialog), new PropertyMetadata(null));


        public bool IsAcceptCommandEnabled
        {
            get { return (bool) GetValue(IsAcceptCommandEnabledProperty); }
            set { SetValue(IsAcceptCommandEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAcceptCommandEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAcceptCommandEnabledProperty =
            DependencyProperty.Register("IsAcceptCommandEnabled", typeof(bool), typeof(CustomDialog), new PropertyMetadata(true));


        public bool IsCancelCommandEnabled
        {
            get { return (bool) GetValue(IsCancelCommandEnabledProperty); }
            set { SetValue(IsCancelCommandEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCancelCommandEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCancelCommandEnabledProperty =
            DependencyProperty.Register("IsCancelCommandEnabled", typeof(bool), typeof(CustomDialog), new PropertyMetadata(true));


        public string AcceptCommandText
        {
            get { return (string) GetValue(AcceptCommandTextProperty); }
            set { SetValue(AcceptCommandTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AccetCommandText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AcceptCommandTextProperty =
            DependencyProperty.Register("AcceptCommandText", typeof(string), typeof(CustomDialog), new PropertyMetadata("Aceptar"));

        public string CancelCommandText
        {
            get { return (string) GetValue(CancelCommandTextProperty); }
            set { SetValue(CancelCommandTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CancelCommandText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelCommandTextProperty =
            DependencyProperty.Register("CancelCommandText", typeof(string), typeof(CustomDialog), new PropertyMetadata("Cancelar"));


        public bool ShowAllCommandButtons
        {
            get { return (bool) GetValue(ShowAllCommandButtonsProperty); }
            set { SetValue(ShowAllCommandButtonsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowAllCommandButtons.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowAllCommandButtonsProperty =
            DependencyProperty.Register("ShowAllCommandButtons", typeof(bool), typeof(CustomDialog), new PropertyMetadata(true));


        public bool IsVisibleAcceptCommand
        {
            get { return (bool) GetValue(IsVisibleAcceptCommandProperty); }
            set { SetValue(IsVisibleAcceptCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsVisibleAcceptCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsVisibleAcceptCommandProperty =
            DependencyProperty.Register("IsVisibleAcceptCommand", typeof(bool), typeof(CustomDialog), new PropertyMetadata(true));

        public bool IsVisibleCancelCommand
        {
            get { return (bool) GetValue(IsVisibleCancelCommandProperty); }
            set { SetValue(IsVisibleCancelCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsVisibleCancelCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsVisibleCancelCommandProperty =
            DependencyProperty.Register("IsVisibleCancelCommand", typeof(bool), typeof(CustomDialog), new PropertyMetadata(true));



        public string AcceptCommandIcon
        {
            get { return (string) GetValue(AcceptCommandIconProperty); }
            set { SetValue(AcceptCommandIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AcceptCommandIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AcceptCommandIconProperty =
            DependencyProperty.Register("AcceptCommandIcon", typeof(string), typeof(CustomDialog), new PropertyMetadata(defaultValue: "\xE8FB"));


        public string CancelCommandIcon
        {
            get { return (string) GetValue(CancelCommandIconProperty); }
            set { SetValue(CancelCommandIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CancelCommandIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelCommandIconProperty =
            DependencyProperty.Register("CancelCommandIcon", typeof(string), typeof(CustomDialog), new PropertyMetadata("\xE711"));



        public FrameworkElement Child
        {
            get { return (FrameworkElement) GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Child.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register("Child", typeof(FrameworkElement), typeof(CustomDialog), new PropertyMetadata(null));




        public Brush AcceptButtonBackground
        {
            get { return (Brush)GetValue(AcceptButtonBackgroundProperty); }
            set { SetValue(AcceptButtonBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AcceptButtoBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AcceptButtonBackgroundProperty =
            DependencyProperty.Register("AcceptButtonBackground", typeof(Brush), typeof(CustomDialog), new PropertyMetadata(null));



        public Brush CancelButtonBackground
        {
            get { return (Brush)GetValue(CancelButtonBackgroundProperty); }
            set { SetValue(CancelButtonBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanceltButtonBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelButtonBackgroundProperty =
            DependencyProperty.Register("CancelButtonBackground", typeof(Brush), typeof(CustomDialog), new PropertyMetadata(null));





        public Brush AcceptButtonForeground
        {
            get { return (Brush)GetValue(AcceptButtonForegroundProperty); }
            set { SetValue(AcceptButtonForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AcceptButtoForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AcceptButtonForegroundProperty =
            DependencyProperty.Register("AcceptButtonForeground", typeof(Brush), typeof(CustomDialog), new PropertyMetadata(null));



        public Brush CancelButtonForeground
        {
            get { return (Brush)GetValue(CancelButtonForegroundProperty); }
            set { SetValue(CancelButtonForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanceltButtonForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelButtonForegroundProperty =
            DependencyProperty.Register("CancelButtonForeground", typeof(Brush), typeof(CustomDialog), new PropertyMetadata(null));

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            dialogService.Close();
        }
    }
}
