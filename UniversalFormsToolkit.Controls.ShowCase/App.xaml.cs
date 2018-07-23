using Microsoft.Practices.Unity;
using Prism.Unity.Windows;
using System.Threading.Tasks;
using UniversalFormsToolkit.Controls.ShowCase.ViewModels;
using UniversalFormsToolkit.Prism.Controls.Interfaces;
using UniversalFormsToolkit.Prism.Controls.Services;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UniversalFormsToolkit.Controls.ShowCase
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : PrismUnityApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
       
        }


        protected override UIElement CreateShell(Frame rootFrame)
        {



            var shell = Container.TryResolve<MainPage>();
         
            return shell;
        }

        protected override async Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
        


        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.RegisterType<IDialogService, DialogService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ItemViewModel>();
        }
    }
}
