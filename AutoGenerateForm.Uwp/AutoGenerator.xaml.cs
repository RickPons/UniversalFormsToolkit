using AutoGenerateForm.Attributes;
using AutoGenerateForm.Uwp.Controls;
using AutoGenerateForm.Uwp.Events;
using AutoGenerateForm.Uwp.Models;
using Callisto.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using WinRTXamlToolkit.Controls.Extensions;
namespace AutoGenerateForm.Uwp
{
    internal class TypeSwitch
    {
        Dictionary<Type, Action<object>> matches = new Dictionary<Type, Action<object>>();
        public TypeSwitch Case<T>(Action<T> action) { matches.Add(typeof(T), (x) => action((T) x)); return this; }
        public void Switch(object x) { matches[x.GetType()](x); }
    }
    public sealed partial class AutoGenerator : UserControl
    {

        private ObservableCollection<Controls.FieldContainerControl> fields = null;

        private bool isLoaded;
        private CoreDispatcher dispatcher = null;
   

        public event EventHandler<FormCreatedEventArgs> OnFormCreated;
        public AutoGenerator()

        {
            this.InitializeComponent();
            if (!DesignMode.DesignModeEnabled)
            {

            
                this.fields = new ObservableCollection<Controls.FieldContainerControl>();
                dispatcher = CoreApplication.GetCurrentView().Dispatcher;
                this.OnFormCreated += AutoGenerator_OnFormCreated;
                this.Unloaded += AutoGenerator_Unloaded;
            }


        }

        private void AutoGenerator_Unloaded(object sender, RoutedEventArgs e)
        {
            if (listView != null && listView.Items != null && listView.Items.Any())

                foreach (var item in listView.Items)
                {
                    var field = item as FieldContainerControl;
                    if (field != null)
                    {
                        var stack = field.Content as StackPanel;
                        if (stack != null && stack.Children != null)
                        {
                            var control = stack.Children.OfType<Control>().FirstOrDefault();
                            if (control != null)
                            {
                                control.KeyDown -= Control_KeyDown;
                            }

                        }
                    }
                  
                }

        }

        public object CurrentDataContext
        {
            get { return (object) GetValue(CurrentDataContextProperty); }
            set { SetValue(CurrentDataContextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentDataContext.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentDataContextProperty =
            DependencyProperty.Register("CurrentDataContext", typeof(object), typeof(AutoGenerator), new PropertyMetadata(null, CurrentDataContextPropertyChanged));

        private static async void CurrentDataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as AutoGenerator;
            if (control == null)
                return;
            if (e.OldValue == null && e.NewValue != null)
            {

                control.GenerateAutoFormControl();
            }
            else
            {
                if (e.OldValue != null && e.NewValue != null)
                {
                    if (e.OldValue.GetType() != e.NewValue.GetType())
                    {

                        control.GenerateAutoFormControl();
                    }
                    else
                    {

                        control.listView.ItemsSource = null;
                        control.listView.ItemsSource = control.fields;

                        foreach (var item in control.listView.Items)
                        {
                            var element = item as Controls.FieldContainerControl;
                            if (element != null)
                            {
                                
                                control.SetVisibilityListViewItem(element);
                                control.RefreshBinding(element, e.NewValue);
                               
                            }

                        }
                        await Task.Delay(control.ValidationDelay);
                        control.UpdateErrorFields(control.ValidationCollection);
                    }

                }
            }

        }

      
        private async void AutoGenerator_OnFormCreated(object sender, FormCreatedEventArgs e)
        {
            await Task.Delay(ValidationDelay);
            isLoaded = true;
            UpdateErrorFields(this.ValidationCollection);

            if (listView != null && listView.Items != null && listView.Items.Any())

                foreach (var item in listView.Items)
                {
                    var field = item as FieldContainerControl;
                    if (field != null)
                    {
                        var stack = field.Content as StackPanel;
                        if (stack != null && stack.Children!=null)
                        {
                            var control = stack.Children.OfType<Control>().FirstOrDefault();
                            if (control != null)
                            {
                                control.KeyDown += Control_KeyDown;
                            }
                            
                        }
                    }
                    SetVisibilityListViewItem(item);
                }



        }

        private void Control_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
          if(e.Key== Windows.System.VirtualKey.Tab)
            {
               // e.Handled = true;
                var originalSource = (Control)e.OriginalSource;
                int index = 0;
                var items = listView.Items;
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        if (originalSource.DataContext == item)
                        {
                            break;
                        }

                        ++index;
                    }

                    index = (index + 1) % items.Count;
                    var nextControl = GetFocusableControl(index);
                    nextControl?.Focus(FocusState.Programmatic);
                   
                }

            }
        }

        private Control GetFocusableControl(int index)
        {
            var container = (ListViewItem)listView.ContainerFromIndex(index);
            var nextControl = FindVisualChild<Control>(container);
            if (nextControl != null)
            {
                var field = nextControl as FieldContainerControl;
                if (field != null)
                {
                    if(field.Visibility== Visibility.Visible)
                    {
                        var stack = field.Content as StackPanel;
                        if (stack != null)
                        {
                            var internalControl = FindVisualChild<Control>(stack);
                            if (internalControl != null)
                            {
                                if (!internalControl.IsEnabled || internalControl.Visibility != Visibility.Visible)
                                {
                                    if (index < listView.Items.Count)
                                    {
                                        index += 1;
                                        return GetFocusableControl(index);
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        if (index < listView.Items.Count)
                        {
                            index += 1;
                            return GetFocusableControl(index);
                        }
                    }

                }
            }
            return nextControl;
           
        }
        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                    T candidate = child as T;
                    if (candidate != null)
                    {
                        return candidate;
                    }

                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }

            return default(T);
        }
        private void SetVisibilityListViewItem(object item)
        {
            var listitem = listView.ContainerFromItem(item) as ListViewItem;
            var element = item as Controls.FieldContainerControl;
            if (element != null && listitem != null)
            {
                var binding0 = new Binding();

                binding0.Source = element;
                binding0.Mode = BindingMode.TwoWay;
                binding0.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                binding0.Path = new PropertyPath("Visibility");
                listitem.SetBinding(ListViewItem.VisibilityProperty, binding0);

            }
        }

        private void RefreshBinding(Control control, object newContext)
        {
            var container = control as AutoGenerateForm.Uwp.Controls.FieldContainerControl;
            if (container == null)
                return;

            var binding0 = new Binding();

            binding0.Source = newContext;
            binding0.Mode = BindingMode.TwoWay;
            binding0.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingExpression bindexpr0 = container.GetBindingExpression(Controls.FieldContainerControl.VisibilityProperty);
            if (bindexpr0 != null && bindexpr0.ParentBinding != null)
            {
                binding0.Converter = new Converters.BooleanToVisibilityConverter();

                binding0.Path = bindexpr0.ParentBinding.Path;
                container.SetBinding(Controls.FieldContainerControl.VisibilityProperty, binding0);
            }
            
            var controls = container.Stack.Children.Where(x => x.GetType().GetTypeInfo().BaseType == typeof(Control) ||
            x.GetType().GetTypeInfo().BaseType == typeof(Selector) ||
            x.GetType().GetTypeInfo().BaseType == typeof(RangeBase) ||
            x.GetType().GetTypeInfo().BaseType == typeof(ToggleButton));
            if (controls == null)
                return;
            foreach (var item in controls)
            {
                var controlType = control.GetType();

                var binding = new Binding();

                binding.Source = newContext;
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                var ts = new TypeSwitch()
            .Case((TextBox x) =>
            {
                BindingExpression bindexpr = x.GetBindingExpression(TextBox.TextProperty);
                if (bindexpr != null && bindexpr.ParentBinding != null)
                {
                    binding.Path = bindexpr.ParentBinding.Path;
                    x.SetBinding(TextBox.TextProperty, binding);
                }


            })
            .Case((ComboBox x) =>
            {

                BindingExpression bindExp = x.GetBindingExpression(ComboBox.SelectedItemProperty);
                Binding bind = bindExp.ParentBinding;

                if (bind != null)
                {
                    var binding2 = new Binding();
                    binding2.Path = bind.Path;
                    binding2.Source = newContext;
                    binding2.Mode = BindingMode.TwoWay;
                    binding2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    x.SetBinding(ComboBox.SelectedItemProperty, binding2);
                }



            })
            .Case((DatePicker x) =>
            {
                BindingExpression bindexpr = x.GetBindingExpression(DatePicker.DateProperty);
                if (bindexpr != null && bindexpr.ParentBinding != null)
                {
                    binding.Converter = new Converters.DateTimeToDateTimeOffsetConverter();
                    binding.Path = bindexpr.ParentBinding.Path;
                    x.SetBinding(DatePicker.DateProperty, binding);
                }

            })
            .Case((AutoSuggestBox x) =>
            {

                BindingExpression bindExp = x.GetBindingExpression(AutoSuggestBox.TextProperty);
                Binding bind = bindExp.ParentBinding;

                if (bind != null)
                {
                    var binding2 = new Binding();
                    binding2.Path = bind.Path;
                    binding2.Source = newContext;
                    binding2.Mode = BindingMode.TwoWay;
                    binding2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    x.SetBinding(AutoSuggestBox.TextProperty, binding2);
                }

                BindingExpression bindexpr = x.GetBindingExpression(AutoSuggestBox.ItemsSourceProperty);
                if (bindexpr != null && bindexpr.ParentBinding != null)
                {
                    binding.Path = bindexpr.ParentBinding.Path;
                    x.SetBinding(AutoSuggestBox.ItemsSourceProperty, binding);
                }

            })
             .Case((NumericUpDown x) =>
             {
                 BindingExpression bindexpr = x.GetBindingExpression(NumericUpDown.ValueProperty);
                 if (bindexpr != null && bindexpr.ParentBinding != null)
                 {
                     binding.Path = bindexpr.ParentBinding.Path;
                     x.SetBinding(NumericUpDown.ValueProperty, binding);
                 }

             })
              .Case((TimePicker x) =>
              {
                  BindingExpression bindexpr = x.GetBindingExpression(TimePicker.TimeProperty  );
                  if (bindexpr != null && bindexpr.ParentBinding != null)
                  {
                      binding.Path = bindexpr.ParentBinding.Path;
                      x.SetBinding(TimePicker.TimeProperty, binding);
                  }

              })
             .Case((CheckBox x) =>
             {

                 BindingExpression bindExp = x.GetBindingExpression(CheckBox.IsCheckedProperty);
                 Binding bind = bindExp.ParentBinding;

                 if (bind != null)
                 {
                     var binding2 = new Binding();
                     binding2.Path = bind.Path;
                     binding2.Source = newContext;
                     binding2.Mode = BindingMode.TwoWay;
                     binding2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                     x.SetBinding(CheckBox.IsCheckedProperty, binding2);
                 }
               
             });


                ts.Switch(item);
            }


        }

        private async void GenerateAutoFormControl()
        {
            await Task.Delay(500);
            fields = new ObservableCollection<Controls.FieldContainerControl>();
            if (IsTitleEnabled)
            {
                TitleTextBlock(this.TitleForm);
            }
            listView.ItemsSource = fields;
           
            //this.Content = scroll;
            this.Content = listView;
            if (this.CurrentDataContext != null)
            {

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    Type mainType = this.CurrentDataContext.GetType();

                    var props = new List<PropertyInfo>(mainType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                                              .Where(x => x.GetCustomAttributes(typeof(AutoGeneratePropertyAttribute), false).Any() &&
                                                                                          x.GetCustomAttributes(typeof(AutoGeneratePropertyAttribute), false)
                                                                                           .Cast<AutoGeneratePropertyAttribute>()
                                                                                           .Any(z => z.AutoGenerate == true))
                                                                              .ToList());
                    var orderedprops = GetOrderedProperties(props);
                    await GenerateForm(orderedprops).ContinueWith(async completed =>
                    {
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            OnFormCreated?.Invoke(this, new FormCreatedEventArgs());
                        });

                    });

                });

            }
        }

        private async Task GenerateForm(List<PropertyInfo> orderedprops, PropertyInfo parentProperty = null)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (orderedprops != null)
                {
                    foreach (var property in orderedprops)
                    {
                        var tColl = typeof(ICollection<>);
                        var propertyType = property.PropertyType;
                        if (!property.PropertyType.Equals(typeof(int)) &&
                            !property.PropertyType.Equals(typeof(string)) &&
                            !property.PropertyType.Equals(typeof(float)) &&
                            !property.PropertyType.Equals(typeof(double)) &&
                            !property.PropertyType.Equals(typeof(decimal)) &&
                            !propertyType.Equals(typeof(Nullable<int>)) &&
                            !propertyType.Equals(typeof(Nullable<float>)) &&
                            !propertyType.Equals(typeof(Nullable<decimal>)) &&
                            !propertyType.Equals(typeof(Nullable<double>)) &&
                            !property.PropertyType.Equals(typeof(DateTime)) &&
                            !property.PropertyType.Equals(typeof(Nullable<DateTime>)) &&
                            !property.PropertyType.Equals(typeof(bool)) &&
                            !propertyType.Equals(typeof(Nullable<bool>)) &&
                            !property.PropertyType.Equals(typeof(TimeSpan)) &&
                            !propertyType.Equals(typeof(Nullable<TimeSpan>)) &&
                            (propertyType.GetTypeInfo().IsGenericType && tColl.IsAssignableFrom(propertyType.GetGenericTypeDefinition()) ||
                             propertyType.GetInterfaces().Any(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == tColl)) == false)
                        {
                            List<PropertyInfo> props = new List<PropertyInfo>(propertyType
                                                                                          .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                                                          .Where(x => x.GetCustomAttributes(typeof(AutoGeneratePropertyAttribute), false).Any() &&
                                                                                                      x.GetCustomAttributes(typeof(AutoGeneratePropertyAttribute), false)
                                                                                                       .Cast<AutoGeneratePropertyAttribute>()
                                                                                                       .Any(z => z.AutoGenerate == true))
                                                                                          .ToList());

                            var orderedprops1 = GetOrderedProperties(props);
                            await GenerateForm(orderedprops1, property);
                        }
                        else
                        {

                            await GenerateControls(property, parentProperty);
                        }
                    }
                }

            });
        }

        private Task GenerateControls(PropertyInfo property, PropertyInfo parentProperty = null)
        {
            var propertyType = property.PropertyType;
            if (propertyType.Equals(typeof(int)) ||
                propertyType.Equals(typeof(long)) ||
                propertyType.Equals(typeof(Nullable<int>)) ||
                propertyType.Equals(typeof(Nullable<long>)))
            {
                return dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var isNumeric = Helpers.AttributeHelper<IsNumericAttribute>.GetAttributeValue(property);
                    if (isNumeric != null)
                    {
                        GenerateNumericUpDown(property, parentProperty);
                    }
                    else
                    {


                        GenerateTextBox(property, parentProperty);
                    }

                }).AsTask();

            }

            if (propertyType.Equals(typeof(string)))
            {
                return dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var isSuggestionsEnabled = Helpers.AttributeHelper<IsSuggestionsEnabledAttribute>.GetAttributeValue(property);
                    if (isSuggestionsEnabled != null)
                    {
                        GenerateSuggestionsControl(property, parentProperty);
                    }
                    else
                    {

                        GenerateTextBox(property, parentProperty);
                    }
                }).AsTask();

            }

            if (propertyType.Equals(typeof(float)) ||
                propertyType.Equals(typeof(decimal)) ||
                propertyType.Equals(typeof(double)) ||
                propertyType.Equals(typeof(Nullable<double>)) ||
                propertyType.Equals(typeof(Nullable<decimal>)) ||
                propertyType.Equals(typeof(Nullable<float>))

            )
            {

                return dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    GenerateNumericUpDown(property, parentProperty);
                }).AsTask();



            }

            var tColl = typeof(ICollection<>);
            if (propertyType.GetTypeInfo().IsGenericType && tColl.IsAssignableFrom(propertyType.GetGenericTypeDefinition()) ||
                propertyType.GetInterfaces().Any(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == tColl))
            {
                return dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    GenerateComboBox(property, parentProperty);
                }).AsTask();



            }
            if (propertyType.Equals(typeof(DateTime)) || propertyType.Equals(typeof(Nullable<DateTime>)))
            {

                return dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    GenerateDatePicker(property, parentProperty);
                }).AsTask();

            }

            if (propertyType.Equals(typeof(bool)) || propertyType.Equals(typeof(Nullable<bool>)))
            {


                return dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    GenerateCheckBox(property, parentProperty);
                }).AsTask();

            }

            if (propertyType.Equals(typeof(TimeSpan)) || propertyType.Equals(typeof(Nullable<TimeSpan>)))
            {

                return dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    GenerateTimePicker(property, parentProperty);
                }).AsTask();



            }
            return null;
        }

        private void GenerateSuggestionsControl(PropertyInfo property, PropertyInfo parentProperty)
        {
            AutoSuggestBox combo = new AutoSuggestBox();
            TextBlock txterror = null;
            var isSugggestionsAttribute = Helpers.AttributeHelper<IsSuggestionsEnabledAttribute>.GetAttributeValue(property);

            //isSugggestionsAttribute.CollectionName Collection to show
            // isSugggestionsAttribute.CollectionName  Display member path
            var binding = new Windows.UI.Xaml.Data.Binding();
            if (parentProperty != null)
            {
                binding.Path = new PropertyPath(parentProperty.Name + "." + isSugggestionsAttribute.CollectionName);
                combo.Name = parentProperty.Name + "_" + isSugggestionsAttribute.CollectionName;
                txterror = GenerateErrorField(combo.Name);
            }


            binding.Source = CurrentDataContext;
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            combo.SetBinding(AutoSuggestBox.ItemsSourceProperty, binding);



            var binding2 = new Windows.UI.Xaml.Data.Binding();

            if (parentProperty != null)
            {
                binding2.Path = new PropertyPath(parentProperty.Name + "." + property.Name);
                binding2.Source = CurrentDataContext;
                binding2.Mode = BindingMode.TwoWay;
                binding2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                // binding2.NotifyOnValidationError = true;

                combo.SetBinding(AutoSuggestBox.TextProperty, binding2);

            }


            TextBlock label = new TextBlock();
            var displayAttribute = Helpers.AttributeHelper<DisplayAttribute>.GetAttributeValue(property);
            if (displayAttribute != null)
            {
                label.Text = displayAttribute.Label;
            }
            else
            {
                label.Text = property.Name;
            }
            object sub = null;
            var subTitleAttribute = Helpers.AttributeHelper<SubtitleAttribute>.GetAttributeValue(property);
            if (subTitleAttribute != null)
            {
                sub = SubTitleTextBlock(subTitleAttribute.SubTitle);

            }
            //CheckIsVisible(combo, parentProperty, property);
            //SetVisibilityBinding(label,combo);
            //SetVisibilityBinding(txterror, combo);
            //stackPanel.Children.Add(label);
            //stackPanel.Children.Add(combo);
            //stackPanel.Children.Add(txterror);


            var field = new Controls.FieldContainerControl();
            if (sub != null)
            {
                field.Stack.Children.Add((TextBlock) sub);
            }
            field.Stack.Children.Add(label);
            field.Stack.Children.Add(combo);
            field.Stack.Children.Add(txterror);
            fields.Add(field);

            CheckIsVisible(field, parentProperty, property);

        }

        private async void GenerateTimePicker(PropertyInfo property, PropertyInfo parentProperty)
        {
            TimePicker num = new TimePicker();

            var minuteAttr = Helpers.AttributeHelper<MinuteIncrementAttribute>.GetAttributeValue(property);
            if (minuteAttr != null)
            {
                 num.MinuteIncrement = minuteAttr.Number;

            }
            var clock = Helpers.AttributeHelper<ClockIdentifierAttribute>.GetAttributeValue(property);
            if (clock!=null)
            {
                num.ClockIdentifier = clock.ClockFormat;
            }
            TextBlock txterror;
            var binding = new Windows.UI.Xaml.Data.Binding();
            if (parentProperty != null)
            {
                binding.Path = new PropertyPath(parentProperty.Name + "." + property.Name);
                num.Name = parentProperty.Name + "_" + property.Name;
                txterror = GenerateErrorField(num.Name);
            }
            else
            {
                binding.Path = new PropertyPath(property.Name);
                num.Name = property.Name;
                txterror = GenerateErrorField(num.Name);
            }
            binding.Source = CurrentDataContext;
            binding.Mode = BindingMode.TwoWay;
            // binding.NotifyOnValidationError = true;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            num.SetBinding(TimePicker.TimeProperty, binding);

            TextBlock label = new TextBlock();
            var displayAttribute = Helpers.AttributeHelper<DisplayAttribute>.GetAttributeValue(property);
            if (displayAttribute != null)
            {
                label.Text = displayAttribute.Label;
            }
            else
            {
                label.Text = property.Name;
            }
            object sub = null;
            var subTitleAttribute = Helpers.AttributeHelper<SubtitleAttribute>.GetAttributeValue(property);
            if (subTitleAttribute != null)
            {
                sub = SubTitleTextBlock(subTitleAttribute.SubTitle);

            }
            var isEnabledAttribute = Helpers.AttributeHelper<IsEnabledPropertyAttribute>.GetAttributeValue(property);
            if (isEnabledAttribute != null)
            {
                if (!string.IsNullOrEmpty(isEnabledAttribute.PropertyToBind))
                {
                    var bindig3 = new Windows.UI.Xaml.Data.Binding();
                    bindig3.Source = CurrentDataContext;

                    if (parentProperty != null)
                    {
                        bindig3.Path = new PropertyPath(parentProperty.Name + "." + isEnabledAttribute.PropertyToBind);
                    }
                    else
                    {
                        bindig3.Path = new PropertyPath(isEnabledAttribute.PropertyToBind);
                    }

                    bindig3.Mode = BindingMode.TwoWay;
                    bindig3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    num.SetBinding(TextBox.IsEnabledProperty, bindig3);
                }
                else
                {
                    if (isEnabledAttribute.IsEnabled)
                    {
                        num.IsEnabled = true;
                    }
                    else
                    {
                        num.IsEnabled = false;
                    }
                }
            }


            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var field = new Controls.FieldContainerControl();
                if (sub != null)
                {
                    field.Stack.Children.Add((TextBlock)sub);
                }
                field.Stack.Children.Add(label);
                field.Stack.Children.Add(num);
                field.Stack.Children.Add(txterror);
                fields.Add(field);
                CheckIsVisible(field, parentProperty, property);
            });

        }



        private async void GenerateDateTimePicker(PropertyInfo property, PropertyInfo parentProperty)
        {
            DatePicker num = new DatePicker();
            TextBlock txterror;
            var binding = new Windows.UI.Xaml.Data.Binding();
            if (parentProperty != null)
            {
                binding.Path = new PropertyPath(parentProperty.Name + "." + property.Name);
                num.Name = parentProperty.Name + "_" + property.Name;
                txterror = GenerateErrorField(num.Name);
            }
            else
            {
                binding.Path = new PropertyPath(property.Name);
                num.Name = property.Name;
                txterror = GenerateErrorField(num.Name);
            }
            binding.Source = CurrentDataContext;
            binding.Mode = BindingMode.TwoWay;
            // binding.NotifyOnValidationError = true;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            num.SetBinding(DatePicker.DateProperty, binding);

            TextBlock label = new TextBlock();
            var displayAttribute = Helpers.AttributeHelper<DisplayAttribute>.GetAttributeValue(property);
            if (displayAttribute != null)
            {
                label.Text = displayAttribute.Label;
            }
            else
            {
                label.Text = property.Name;
            }
            object sub = null;
            var subTitleAttribute = Helpers.AttributeHelper<SubtitleAttribute>.GetAttributeValue(property);
            if (subTitleAttribute != null)
            {
                sub = SubTitleTextBlock(subTitleAttribute.SubTitle);

            }
            var isEnabledAttribute = Helpers.AttributeHelper<IsEnabledPropertyAttribute>.GetAttributeValue(property);
            if (isEnabledAttribute != null)
            {
                if (!string.IsNullOrEmpty(isEnabledAttribute.PropertyToBind))
                {
                    var bindig3 = new Windows.UI.Xaml.Data.Binding();
                    bindig3.Source = CurrentDataContext;

                    if (parentProperty != null)
                    {
                        bindig3.Path = new PropertyPath(parentProperty.Name + "." + isEnabledAttribute.PropertyToBind);
                    }
                    else
                    {
                        bindig3.Path = new PropertyPath(isEnabledAttribute.PropertyToBind);
                    }

                    bindig3.Mode = BindingMode.TwoWay;
                    bindig3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    num.SetBinding(TextBox.IsEnabledProperty, bindig3);
                }
                else
                {
                    if (isEnabledAttribute.IsEnabled)
                    {
                        num.IsEnabled = true;
                    }
                    else
                    {
                        num.IsEnabled = false;
                    }
                }
            }
      

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var field = new Controls.FieldContainerControl();
                if (sub != null)
                {
                    field.Stack.Children.Add((TextBlock)sub);
                }
                field.Stack.Children.Add(label);
                field.Stack.Children.Add(num);
                field.Stack.Children.Add(txterror);
                fields.Add(field);
                CheckIsVisible(field, parentProperty, property);
            });

        }

        private async void GenerateComboBox(PropertyInfo property, PropertyInfo parentProperty = null)
        {
            ComboBox combo = new ComboBox();
            TextBlock txterror;
            var binding = new Windows.UI.Xaml.Data.Binding();
            if (parentProperty != null)
            {
                binding.Path = new PropertyPath(parentProperty.Name + "." + property.Name);
                combo.Name = parentProperty.Name + "_" + property.Name;
                txterror = GenerateErrorField(combo.Name);
            }
            else
            {
                binding.Path = new PropertyPath(property.Name);
                combo.Name = property.Name;
                txterror = GenerateErrorField(combo.Name);
            }
            binding.Source = CurrentDataContext;
            binding.Mode = BindingMode.TwoWay;
            // binding.NotifyOnValidationError = true;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            combo.SetBinding(ComboBox.ItemsSourceProperty, binding);
            var displayMemberPathAttribute = Helpers.AttributeHelper<DisplayMemberPathCollectionAttribute>.GetAttributeValue(property);
            if (displayMemberPathAttribute != null)
            {
                combo.DisplayMemberPath = displayMemberPathAttribute.DisplayMemberPath;
            }
            var selectedItemAttribute = Helpers.AttributeHelper<SelectedItemCollectionAttribute>.GetAttributeValue(property);
            if (selectedItemAttribute != null)
            {
                var binding2 = new Windows.UI.Xaml.Data.Binding();

                if (parentProperty != null)
                {
                    binding2.Path = new PropertyPath(parentProperty.Name + "." + selectedItemAttribute.PropertyNameToBind);
                }
                else
                {
                    binding2.Path = new PropertyPath(selectedItemAttribute.PropertyNameToBind);
                }
                binding2.Source = CurrentDataContext;
                binding2.Mode = BindingMode.TwoWay;
                binding2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                //  binding2.NotifyOnValidationError = true;

                combo.SetBinding(ComboBox.SelectedItemProperty, binding2);
            }

            var isEnabledAttribute = Helpers.AttributeHelper<IsEnabledPropertyAttribute>.GetAttributeValue(property);
            if (isEnabledAttribute != null)
            {
                if (!string.IsNullOrEmpty(isEnabledAttribute.PropertyToBind))
                {
                    var bindig3 = new Windows.UI.Xaml.Data.Binding();
                    bindig3.Source = CurrentDataContext;

                    if (parentProperty != null)
                    {
                        bindig3.Path = new PropertyPath(parentProperty.Name + "." + isEnabledAttribute.PropertyToBind);
                    }
                    else
                    {
                        bindig3.Path = new PropertyPath(isEnabledAttribute.PropertyToBind);
                    }

                    bindig3.Mode = BindingMode.TwoWay;
                    bindig3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    combo.SetBinding(TextBox.IsEnabledProperty, bindig3);
                }
                else
                {
                    if (isEnabledAttribute.IsEnabled)
                    {
                        combo.IsEnabled = true;
                    }
                    else
                    {
                        combo.IsEnabled = false;
                    }
                }
            }
            var label = new TextBlock();
            var displayAttribute = Helpers.AttributeHelper<DisplayAttribute>.GetAttributeValue(property);
            if (displayAttribute != null)
            {
                label.Text = displayAttribute.Label;
            }
            else
            {
                label.Text = property.Name;
            }
            object sub = null;
            var subTitleAttribute = Helpers.AttributeHelper<SubtitleAttribute>.GetAttributeValue(property);
            if (subTitleAttribute != null)
            {
                sub = SubTitleTextBlock(subTitleAttribute.SubTitle);

            }

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var field = new Controls.FieldContainerControl();
                if (sub != null)
                {
                    field.Stack.Children.Add((TextBlock) sub);
                }
                field.Stack.Children.Add(label);
                field.Stack.Children.Add(combo);
                field.Stack.Children.Add(txterror);
                fields.Add(field);
                CheckIsVisible(field, parentProperty, property);
            });
            //CheckIsVisible(combo, parentProperty, property);
            //SetVisibilityBinding(label, combo);
            //SetVisibilityBinding(txterror, combo);
            //stackPanel.Children.Add(label);
            //stackPanel.Children.Add(combo);
            //stackPanel.Children.Add(txterror);


        }

        private async void GenerateNumericUpDown(PropertyInfo property, PropertyInfo parentProperty = null)
        {
            var num = new Callisto.Controls.NumericUpDown();
            var binding = new Binding();
            TextBlock txterror;

            var decimalCount = Helpers.AttributeHelper<DecimalCountAttribute>.GetAttributeValue(property);
            if (decimalCount != null)
            {
                num.DecimalPlaces = decimalCount.Number;
            }


            var autoincrement = Helpers.AttributeHelper<AutoIncrementAttribute>.GetAttributeValue(property);
            if (autoincrement != null)
            {
                if (autoincrement.Step <= 0)
                {
                    num.Increment = 1;
                }
                else
                {
                    num.Increment = (double) autoincrement.Step;
                }

            }
            var range = Helpers.AttributeHelper<RangeAttribute>.GetAttributeValue(property);
            if (range != null)
            {
                num.Minimum = range.Min;
                num.Maximum = range.Max;
            }

            if (parentProperty != null)
            {
                binding.Path = new PropertyPath(parentProperty.Name + "." + property.Name);
                num.Name = parentProperty.Name + "_" + property.Name;
                txterror = GenerateErrorField(num.Name);
            }
            else
            {
                binding.Path = new PropertyPath(property.Name);
                num.Name = property.Name;
                txterror = GenerateErrorField(num.Name);
            }
            binding.Source = CurrentDataContext;
            binding.Mode = BindingMode.TwoWay;
            // binding.NotifyOnValidationError = true;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            num.SetBinding(Callisto.Controls.NumericUpDown.ValueProperty, binding);

            
            var isEnabledAttribute = Helpers.AttributeHelper<IsEnabledPropertyAttribute>.GetAttributeValue(property);
            if (isEnabledAttribute != null)
            {
                if (!string.IsNullOrEmpty(isEnabledAttribute.PropertyToBind))
                {
                    Binding bindig3 = new Binding();
                    bindig3.Source = CurrentDataContext;

                    if (parentProperty != null)
                    {
                        bindig3.Path = new PropertyPath(parentProperty.Name + "." + isEnabledAttribute.PropertyToBind);
                    }
                    else
                    {
                        bindig3.Path = new PropertyPath(isEnabledAttribute.PropertyToBind);
                    }

                    bindig3.Mode = BindingMode.TwoWay;
                    bindig3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    num.SetBinding(TextBox.IsEnabledProperty, bindig3);
                }
                else
                {
                    if (isEnabledAttribute.IsEnabled)
                    {
                        num.IsEnabled = true;
                    }
                    else
                    {
                        num.IsEnabled = false;
                    }
                }
            }
            var label = new TextBlock();
            var displayAttribute = Helpers.AttributeHelper<DisplayAttribute>.GetAttributeValue(property);
            if (displayAttribute != null)
            {
                label.Text = displayAttribute.Label;
            }
            else
            {
                label.Text = property.Name;
            }
            object sub = null;
            var subTitleAttribute = Helpers.AttributeHelper<SubtitleAttribute>.GetAttributeValue(property);
            if (subTitleAttribute != null)
            {
                sub = SubTitleTextBlock(subTitleAttribute.SubTitle);

            }

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var field = new Controls.FieldContainerControl();
                if (sub != null)
                {
                    field.Stack.Children.Add((TextBlock)sub);
                }
                field.Stack.Children.Add(label);
                field.Stack.Children.Add(num);
                field.Stack.Children.Add(txterror);
                fields.Add(field);
                CheckIsVisible(field, parentProperty, property);
            });

            // CheckIsVisible(num, parentProperty, property);
            //stackPanel.Children.Add(label);
            //stackPanel.Children.Add(num);
            //stackPanel.Children.Add(txterror);
        }


        private async void GenerateTextBox(PropertyInfo property, PropertyInfo parentProperty = null)
        {
            var txt = new TextBox();
            TextBlock txterror;
            txt.TextChanged += Txt_TextChanged;
            txt.TextWrapping = TextWrapping.Wrap;

            var binding = new Binding();
            if (parentProperty != null)
            {
                binding.Path = new PropertyPath(parentProperty.Name + "." + property.Name);
                txt.Name = parentProperty.Name + "_" + property.Name;
                txterror = GenerateErrorField(txt.Name);
            }
            else
            {
                binding.Path = new PropertyPath(property.Name);
                txt.Name = property.Name;
                txterror = GenerateErrorField(txt.Name);
            }
            binding.Source = CurrentDataContext;
            binding.Mode = BindingMode.TwoWay;
            // binding.NotifyOnValidationError = true;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            txt.SetBinding(TextBox.TextProperty, binding);
            var label = new TextBlock();
            var multilineAttribute = Helpers.AttributeHelper<MultilineAttribute>.GetAttributeValue(property);
            if (multilineAttribute != null)
            {
                txt.AcceptsReturn = true;
            }
            var displayAttribute = Helpers.AttributeHelper<DisplayAttribute>.GetAttributeValue(property);
            if (displayAttribute != null)
            {
                label.Text = displayAttribute.Label;
            }
            else
            {
                label.Text = property.Name;
            }
            var minAttribute = Helpers.AttributeHelper<MinMaxSizeAttribute>.GetAttributeValue(property);
            if (minAttribute != null)
            {
                if (minAttribute.MinWidth > 0)
                    txt.MinWidth = minAttribute.MinWidth;
                if (minAttribute.MinHeight > 0)
                    txt.MinHeight = minAttribute.MinHeight;
                if (minAttribute.MaxHeight > 0)
                    txt.MaxHeight = minAttribute.MaxHeight;
                if (minAttribute.MaxWidth > 0)
                    txt.MaxWidth = minAttribute.MaxWidth;
            }
            var stringLinght = Helpers.AttributeHelper<StringLengthAttribute>.GetAttributeValue(property);
            if (stringLinght != null)
            {
                txt.MaxLength = stringLinght.Count;
            }
            var isEnabledAttribute = Helpers.AttributeHelper<IsEnabledPropertyAttribute>.GetAttributeValue(property);
            if (isEnabledAttribute != null)
            {
                if (!string.IsNullOrEmpty(isEnabledAttribute.PropertyToBind))
                {
                    Binding bindig3 = new Binding();
                    bindig3.Source = CurrentDataContext;

                    if (parentProperty != null)
                    {
                        bindig3.Path = new PropertyPath(parentProperty.Name + "." + isEnabledAttribute.PropertyToBind);
                    }
                    else
                    {
                        bindig3.Path = new PropertyPath(isEnabledAttribute.PropertyToBind);
                    }

                    bindig3.Mode = BindingMode.TwoWay;
                    bindig3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    txt.SetBinding(TextBox.IsEnabledProperty, bindig3);
                }
                else
                {
                    if (isEnabledAttribute.IsEnabled)
                    {
                        txt.IsEnabled = true;
                    }
                    else
                    {
                        txt.IsEnabled = false;
                    }
                }
            }
            object sub = null;
            var subTitleAttribute = Helpers.AttributeHelper<SubtitleAttribute>.GetAttributeValue(property);
            if (subTitleAttribute != null)
            {
                sub = SubTitleTextBlock(subTitleAttribute.SubTitle);

            }

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var field = new Controls.FieldContainerControl();
                if (sub != null)
                {
                    field.Stack.Children.Add((TextBlock) sub);
                }
                field.Stack.Children.Add(label);
                field.Stack.Children.Add(txt);
                field.Stack.Children.Add(txterror);
                CheckIsVisible(field, parentProperty, property);
                fields.Add(field);


            });


        }

        private void Txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsUpperCaseEnabled)
                return;
            var text = sender as TextBox;
            if (text != null && text.Text.Any())
            {
                try
                {
                    var selectionStart = text.SelectionStart;
                    var selectionLengh = text.SelectionLength;
                    text.Text = text.Text.ToUpper();
                    text.SelectionStart = selectionStart;
                    text.SelectionLength = selectionLengh;
                }
                catch (Exception)
                {


                }

            }
        }

        private async void GenerateDatePicker(PropertyInfo property, PropertyInfo parentProperty = null)
        {
            DatePicker picker = new DatePicker();
            TextBlock txterror;
            Binding binding = new Binding();
            if (parentProperty != null)
            {
                binding.Path = new PropertyPath(parentProperty.Name + "." + property.Name);
                picker.Name = parentProperty.Name + "_" + property.Name;
                txterror = GenerateErrorField(picker.Name);
            }
            else
            {
                binding.Path = new PropertyPath(property.Name);
                picker.Name = property.Name;
                txterror = GenerateErrorField(picker.Name);
            }
            binding.Converter = new Converters.DateTimeToDateTimeOffsetConverter();
            binding.Source = CurrentDataContext;
            binding.Mode = BindingMode.TwoWay;
            //  binding.NotifyOnValidationError = true;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            picker.SetBinding(DatePicker.DateProperty, binding);
            var isEnabledAttribute = Helpers.AttributeHelper<IsEnabledPropertyAttribute>.GetAttributeValue(property);
            if (isEnabledAttribute != null)
            {
                if (!string.IsNullOrEmpty(isEnabledAttribute.PropertyToBind))
                {
                    Binding bindig3 = new Binding();
                    bindig3.Source = CurrentDataContext;

                    if (parentProperty != null)
                    {
                        bindig3.Path = new PropertyPath(parentProperty.Name + "." + isEnabledAttribute.PropertyToBind);
                    }
                    else
                    {
                        bindig3.Path = new PropertyPath(isEnabledAttribute.PropertyToBind);
                    }

                    bindig3.Mode = BindingMode.TwoWay;
                    bindig3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    picker.SetBinding(TextBox.IsEnabledProperty, bindig3);
                }
                else
                {
                    if (isEnabledAttribute.IsEnabled)
                    {
                        picker.IsEnabled = true;
                    }
                    else
                    {
                        picker.IsEnabled = false;
                    }
                }
            }
            TextBlock label = new TextBlock();
            var displayAttribute = Helpers.AttributeHelper<DisplayAttribute>.GetAttributeValue(property);
            if (displayAttribute != null)
            {
                label.Text = displayAttribute.Label;
            }
            else
            {
                label.Text = property.Name;
            }

            object sub = null;
            var subTitleAttribute = Helpers.AttributeHelper<SubtitleAttribute>.GetAttributeValue(property);
            if (subTitleAttribute != null)
            {
                sub = SubTitleTextBlock(subTitleAttribute.SubTitle);

            }

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var field = new Controls.FieldContainerControl();
                if (sub != null)
                {
                    field.Stack.Children.Add((TextBlock) sub);
                }
                field.Stack.Children.Add(label);
                field.Stack.Children.Add(picker);
                field.Stack.Children.Add(txterror);
                fields.Add(field);
                CheckIsVisible(field, parentProperty, property);
            });


        }

        private async void GenerateCheckBox(PropertyInfo property, PropertyInfo parentProperty = null)
        {
            var box = new CheckBox();
            TextBlock txterror;
            Binding binding = new Binding();
            if (parentProperty != null)
            {
                binding.Path = new PropertyPath(parentProperty.Name + "." + property.Name);
                box.Name = parentProperty.Name + "_" + property.Name;
                txterror = GenerateErrorField(box.Name);
            }
            else
            {
                binding.Path = new PropertyPath(property.Name);
                box.Name = property.Name;
                txterror = GenerateErrorField(box.Name);
            }
            binding.Source = CurrentDataContext;
            binding.Mode = BindingMode.TwoWay;
            //  binding.NotifyOnValidationError = true;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            box.SetBinding(CheckBox.IsCheckedProperty, binding);
            var isEnabledAttribute = Helpers.AttributeHelper<IsEnabledPropertyAttribute>.GetAttributeValue(property);
            if (isEnabledAttribute != null)
            {
                if (!string.IsNullOrEmpty(isEnabledAttribute.PropertyToBind))
                {
                    Binding bindig3 = new Binding();
                    bindig3.Source = CurrentDataContext;

                    if (parentProperty != null)
                    {
                        bindig3.Path = new PropertyPath(parentProperty.Name + "." + isEnabledAttribute.PropertyToBind);
                    }
                    else
                    {
                        bindig3.Path = new PropertyPath(isEnabledAttribute.PropertyToBind);
                    }

                    bindig3.Mode = BindingMode.TwoWay;
                    bindig3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    box.SetBinding(TextBox.IsEnabledProperty, bindig3);
                }
                else
                {
                    if (isEnabledAttribute.IsEnabled)
                    {
                        box.IsEnabled = true;
                    }
                    else
                    {
                        box.IsEnabled = false;
                    }
                }
            }
            TextBlock label = new TextBlock();
            var displayAttribute = Helpers.AttributeHelper<DisplayAttribute>.GetAttributeValue(property);
            if (displayAttribute != null)
            {
                label.Text = displayAttribute.Label;
            }
            else
            {
                label.Text = property.Name;
            }
            object sub = null;
            var subTitleAttribute = Helpers.AttributeHelper<SubtitleAttribute>.GetAttributeValue(property);
            if (subTitleAttribute != null)
            {
                 sub = SubTitleTextBlock(subTitleAttribute.SubTitle);
                
            }
         

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var field = new Controls.FieldContainerControl();
                if(sub!=null)

                {
                    field.Stack.Children.Add((TextBlock)sub);
                }
                field.Stack.Children.Add(label);
                field.Stack.Children.Add(box);
                field.Stack.Children.Add(txterror);
                fields.Add(field);
                CheckIsVisible(field, parentProperty, property);
            });



        }


        private List<PropertyInfo> GetOrderedProperties(List<PropertyInfo> properties)
        {
            var list = new List<OrderPropertyObject>();
            foreach (var property in properties)
            {
                var orderAttribute = Helpers.AttributeHelper<PropertyOrderAttribute>.GetAttributeValue(property);
                var obj = new OrderPropertyObject();
                if (orderAttribute != null)
                {
                    obj.Order = orderAttribute.Order;
                    obj.Property = property;
                }
                else
                {
                    obj.Property = property;
                }

                list.Add(obj);
            }

            var orderedList = list.OrderBy(x => x.Order).Select(y => new { y.Property }).ToList();
            var result = new List<PropertyInfo>();
            foreach (var item in orderedList)
            {
                result.Add(item.Property);
            }
            return result;
        }

        private TextBlock SubTitleTextBlock(string subtitle)
        {
            if (!string.IsNullOrEmpty(subtitle))
            {
                TextBlock txt = new TextBlock();
                txt.Text = subtitle.ToUpper();
                txt.FontWeight = FontWeights.Bold;
                txt.FontSize = 14;
                txt.Margin = new Thickness(0, 5, 0, 5);
                return txt;
            }

            return null;
        }

        private void TitleTextBlock(string title)
        {
            if (!string.IsNullOrEmpty(title) && fields!= null)
            {
                TextBlock txt = new TextBlock();
                txt.Text = title.ToUpper();
                txt.FontWeight = FontWeights.Bold;
                txt.FontSize = 18;
                txt.Margin = new Thickness(0, 5, 0, 5);
                fields.Add(new Controls.FieldContainerControl() { Content = txt });
            }
        }
        private void SetVisibilityBinding(TextBlock txt, Control control)
        {
            if (txt != null)
            {
                Binding bindig3 = new Binding();
                bindig3.Source = control;
                bindig3.Path = new PropertyPath("Visibility");
                bindig3.Mode = BindingMode.TwoWay;
                bindig3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                txt.SetBinding(TextBlock.VisibilityProperty, bindig3);
            }
        }

        private void CheckIsVisible(Controls.FieldContainerControl control, PropertyInfo parentProperty, PropertyInfo property)
        {
            if (control == null)
                return;




            var isVisibleAttribute = Helpers.AttributeHelper<IsVisibleAttribute>.GetAttributeValue(property);
            if (isVisibleAttribute != null)
            {
                Binding bindig3 = new Binding();
                if (isVisibleAttribute != null && !string.IsNullOrEmpty(isVisibleAttribute.PropertyNameToBind))
                {

                    bindig3.Source = CurrentDataContext;

                    if (parentProperty != null)
                    {
                        bindig3.Path = new PropertyPath(parentProperty.Name + "." + isVisibleAttribute.PropertyNameToBind);
                    }
                    else
                    {
                        bindig3.Path = new PropertyPath(isVisibleAttribute.PropertyNameToBind);
                    }
                    bindig3.Converter = new Converters.BooleanToVisibilityConverter();
                    bindig3.Mode = BindingMode.TwoWay;
                    bindig3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    control.SetBinding(Controls.FieldContainerControl.VisibilityProperty, bindig3);
                }
            }


        }

        public TimeSpan ValidationDelay
        {
            get { return (TimeSpan) GetValue(ValidationDelayProperty); }
            set { SetValue(ValidationDelayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValidationDelay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValidationDelayProperty =
            DependencyProperty.Register("ValidationDelay", typeof(TimeSpan), typeof(AutoGenerator), new PropertyMetadata(TimeSpan.FromSeconds(1)));


        public string TitleForm
        {
            get
            {
                return (string) GetValue(TitleFormProperty);
            }
            set
            {
                SetValue(TitleFormProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for TitleForm.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleFormProperty =
            DependencyProperty.Register("TitleForm", typeof(string), typeof(AutoGenerator), new PropertyMetadata(string.Empty, TitleFormPropertyChanged));

        private static void TitleFormPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as AutoGenerator;
            if (control != null)
            {
                if (control.IsTitleEnabled && e.NewValue != null && !string.IsNullOrEmpty(e.NewValue.ToString()))
                {
                    control.TitleTextBlock(e.NewValue.ToString());
                }
            }
        }

        public bool IsTitleEnabled
        {
            get
            {
                return (bool) GetValue(IsTitleEnabledProperty);
            }
            set
            {
                SetValue(IsTitleEnabledProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for IsTitleEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTitleEnabledProperty =
            DependencyProperty.Register("IsTitleEnabled", typeof(bool), typeof(AutoGenerator), new PropertyMetadata(false));



        public bool IsUpperCaseEnabled
        {
            get { return (bool) GetValue(IsUpperCaseEnabledProperty); }
            set { SetValue(IsUpperCaseEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for isUpperCaseEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUpperCaseEnabledProperty =
            DependencyProperty.Register("IsUpperCaseEnabled", typeof(bool), typeof(AutoGenerator), new PropertyMetadata(true));



        public ICollection<ValidationModel> ValidationCollection
        {
            get { return (ICollection<ValidationModel>) GetValue(ValidationCollectionProperty); }
            set { SetValue(ValidationCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValidationCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValidationCollectionProperty =
            DependencyProperty.Register("ValidationCollection", typeof(ICollection<ValidationModel>), typeof(AutoGenerator), new PropertyMetadata(null, ValidationCollectionPropertyChanged));

        private static void ValidationCollectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as AutoGenerator;
            if (control == null)
                return;
            if (control.isLoaded)
            {
                var collection = e.NewValue as ICollection<ValidationModel>;
                control.UpdateErrorFields(collection);
            }


        }

        private TextBlock GenerateErrorField(string name)
        {
            var block = new TextBlock();
            block.Name = name + "_Error";
            block.Foreground = new SolidColorBrush(Colors.Red);
            return block;
        }

        internal void UpdateErrorFields(ICollection<ValidationModel> collection)
        {
            var textBlocks = listView.GetDescendantsOfType<TextBlock>();
            var errorBlocks = textBlocks.Where(x => x.Name.Contains("_Error"));
            foreach (var item in errorBlocks)
            {
                item.Text = string.Empty;
            }
            if (collection != null)
            {
                foreach (var item in collection)
                {
                    var nameToFind = string.Empty;
                    if (!string.IsNullOrEmpty(item.ParentPropertyName))
                    {
                        nameToFind = item.ParentPropertyName + "_" + item.PropertyName + "_Error";
                    }
                    else
                    {
                        nameToFind = item.PropertyName + "_Error";
                    }

                    if (nameToFind != null)
                    {
                        var element = listView.FindName(nameToFind);
                        if (element != null)
                        {
                            var txt = element as TextBlock;
                            txt.Text = item.ErrorMessage;
                        }
                    }
                }
            }
        }
    }
}