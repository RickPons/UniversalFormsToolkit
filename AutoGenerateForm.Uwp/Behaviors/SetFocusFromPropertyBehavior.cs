using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;
using WinRTXamlToolkit.Controls.Extensions;

namespace AutoGenerateForm.Uwp.Behaviors
{
    internal class SetFocusFromPropertyBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        public string PropertyToSetFocus
        {
            get
            {
                return (string) GetValue(PropertyToSetFocusProperty);
            }
            set
            {
                SetValue(PropertyToSetFocusProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for PropertyToSetFocus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyToSetFocusProperty =
            DependencyProperty.Register("PropertyToSetFocus", typeof(string), typeof(SetFocusFromPropertyBehavior), new PropertyMetadata(string.Empty, PropertyToSetFocusPropertyChanged));

        private static void PropertyToSetFocusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SetFocusFromPropertyBehavior;
            if (control != null && e.NewValue != null)
            {
                var nameToFind = string.Empty;
                if (!string.IsNullOrEmpty(control.ParentPropertyToSetFocus))
                {
                    nameToFind = control.ParentPropertyToSetFocus + "_" + e.NewValue.ToString();
                }
                else
                {
                    nameToFind = e.NewValue.ToString();
                }

                var controlToSetFocus = control.AssociatedObject.FindName(nameToFind);
                if (controlToSetFocus != null)
                {
                    var c = controlToSetFocus as Control;
                    if (c != null)
                    {
                        AutoGenerateForm.Uwp.AutoGenerator generator = null;
                        if (control.AssociatedObject is AutoGenerateForm.Uwp.AutoGenerator)
                        {
                            generator = (AutoGenerateForm.Uwp.AutoGenerator) control.AssociatedObject;
                        }
                        else
                        {
                            generator = control.AssociatedObject.GetFirstDescendantOfType<AutoGenerateForm.Uwp.AutoGenerator>();

                        }

                        c.Focus(FocusState.Pointer);
                        if (generator == null)
                        {
                            Debug.WriteLine(" auto generator not found");
                            return;
                        }
                        else
                        {
                            control.SetViewForItem(c, generator);
                        }

                    }
                }
            }
        }

        public string ParentPropertyToSetFocus
        {
            get
            {
                return (string) GetValue(ParentPropertyToSetFocusProperty);
            }
            set
            {
                SetValue(ParentPropertyToSetFocusProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ParentPropertyToSetFocus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentPropertyToSetFocusProperty =
            DependencyProperty.Register("ParentPropertyToSetFocus", typeof(string), typeof(SetFocusFromPropertyBehavior), new PropertyMetadata(string.Empty));

        internal async Task SetViewForItem(FrameworkElement item, AutoGenerateForm.Uwp.AutoGenerator generator)
        {
            var listView = generator.GetFirstDescendantOfType<ListView>();

            var scrollViewer = listView.GetFirstDescendantOfType<ScrollViewer>();
            if (scrollViewer == null)
            {
                Debug.WriteLine(" scrollviewer not found");
                return;
            }
            ListViewItem listViewItem = null;
            var children = listView.GetDescendantsOfType<ListViewItem>();
            if (children == null)
            {
                Debug.WriteLine(" listview items not found");
                return;
            }
            foreach (var child in children)
            {
                var c = child.FindName(item.Name);
                if(c!=null)
                {
                    listViewItem = child;
                    break;
                }
               

            }
            // Calculations relative to screen or ListView
          
           




            if (listViewItem == null)
            {
                Debug.WriteLine(" child element not found");
                return;
            }


            var topLeft =
                listViewItem
                    .TransformToVisual(generator)
                    .TransformPoint(new Point()).Y;
            var lvih = listViewItem.ActualHeight;
            var lvh = generator.ActualHeight;
            var desiredTopLeft = (lvh - lvih) / 2.0;
            var desiredDelta = topLeft - desiredTopLeft;

            // Calculations relative to the ScrollViewer within the ListView

            var currentOffset = scrollViewer.VerticalOffset;
            var desiredOffset = currentOffset + desiredDelta;


            scrollViewer.ChangeView(null, desiredOffset, null);
        }

    }
}
