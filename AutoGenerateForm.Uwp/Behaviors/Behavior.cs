using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;

namespace AutoGenerateForm.Uwp.Behaviors
{
    /// <summary>
    /// If you like to use behaviors like WPF or Silverlight, you can use this base class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Behavior<T> : DependencyObject, IAction, IBehavior where T : DependencyObject
    {

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public T AssociatedObject { get; set; }

        protected virtual void OnAttached()
        {
        }

        protected virtual void OnDetaching()
        {
        }

        public void Attach(Windows.UI.Xaml.DependencyObject associatedObject)
        {
            this.AssociatedObject = (T) associatedObject;
            OnAttached();
        }

        public void Detach()
        {
            OnDetaching();
        }

        public object Execute(object sender, object parameter)
        {
            return null;
        }

        DependencyObject IBehavior.AssociatedObject
        {
            get { return this.AssociatedObject; }
        }
    }
}
