using AutoGenerateForm.Attributes;
using AutoGenerateForm.Uwp.Events;
using AutoGenerateForm.Uwp.Models;
using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using WinRTXamlToolkit.Controls;
using WinRTXamlToolkit.Controls.Extensions;
namespace AutoGenerateForm.Uwp
{
    public class AutoGenerator : UserControl
    {
        private StackPanel stackPanel = null;
        private bool isLoaded;
        private CoreDispatcher dispatcher = null;

        public event EventHandler<FormCreatedEventArgs> OnFormCreated;
        public AutoGenerator()

        {
            if (!DesignMode.DesignModeEnabled)
            {
                this.Loaded += AutoGenerator_Loaded1;
                this.stackPanel = new StackPanel();
                dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                this.OnFormCreated += AutoGenerator_OnFormCreated;
            }


        }

        private async void AutoGenerator_OnFormCreated(object sender, FormCreatedEventArgs e)
        {
            await Task.Delay(1000);
            isLoaded = true;
            UpdateErrorFields(this.ValidationCollection);
        }

        private void AutoGenerator_Loaded1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (isLoaded)
                return;
            GenerateAutoFormControl();
        }



        private async void GenerateAutoFormControl()
        {
            await Task.Delay(500);
            ScrollViewer scroll = new ScrollViewer();
            scroll.HorizontalAlignment = HorizontalAlignment.Stretch;
            scroll.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            scroll.VerticalAlignment = VerticalAlignment.Stretch;
            scroll.VerticalContentAlignment = VerticalAlignment.Stretch;
            scroll.HorizontalScrollMode = ScrollMode.Disabled;
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scroll.HorizontalScrollMode = ScrollMode.Disabled;
            scroll.VerticalScrollMode = ScrollMode.Auto;
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            stackPanel.Margin = new Thickness(0, 0, 0, 24);
            scroll.Content = stackPanel;
            stackPanel.Children.Clear();

            this.Content = scroll;
            if (this.DataContext != null)
            {

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                 {
                     Type mainType = this.DataContext.GetType();

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

        private Task GenerateForm(List<PropertyInfo> orderedprops, PropertyInfo parentProperty = null)
        {
            return Task.Factory.StartNew( async() =>
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
                          await  GenerateControls(property, parentProperty);
                        }
                    }
                }
                else
                {

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
                    GenerateDateTimePicker(property, parentProperty);
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


            binding.Source = this.DataContext;
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            combo.SetBinding(AutoSuggestBox.ItemsSourceProperty, binding);



            var binding2 = new Windows.UI.Xaml.Data.Binding();

            if (parentProperty != null)
            {
                binding2.Path = new PropertyPath(parentProperty.Name + "." + property.Name);
                binding2.Source = this.DataContext;
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
            var subTitleAttribute = Helpers.AttributeHelper<SubtitleAttribute>.GetAttributeValue(property);
            if (subTitleAttribute != null)
            {
                var sub = SubTitleTextBlock(subTitleAttribute.SubTitle);
                if (sub != null)
                {
                    stackPanel.Children.Add(sub);
                }
            }
            stackPanel.Children.Add(label);
            stackPanel.Children.Add(combo);
            stackPanel.Children.Add(txterror);
        }



        private void GenerateDateTimePicker(PropertyInfo property, PropertyInfo parentProperty)
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
            binding.Source = this.DataContext;
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
            var subTitleAttribute = Helpers.AttributeHelper<SubtitleAttribute>.GetAttributeValue(property);
            if (subTitleAttribute != null)
            {
                var sub = SubTitleTextBlock(subTitleAttribute.SubTitle);
                if (sub != null)
                {
                    stackPanel.Children.Add(sub);
                }
            }
            var isEnabledAttribute = Helpers.AttributeHelper<IsEnabledPropertyAttribute>.GetAttributeValue(property);
            if (isEnabledAttribute != null)
            {
                if (!string.IsNullOrEmpty(isEnabledAttribute.PropertyToBind))
                {
                    var bindig3 = new Windows.UI.Xaml.Data.Binding();
                    bindig3.Source = this.DataContext;

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
            stackPanel.Children.Add(label);
            stackPanel.Children.Add(num);
            stackPanel.Children.Add(txterror);
        }

        private void GenerateComboBox(PropertyInfo property, PropertyInfo parentProperty = null)
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
            binding.Source = this.DataContext;
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
                binding2.Source = this.DataContext;
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
                    bindig3.Source = this.DataContext;

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
            var subTitleAttribute = Helpers.AttributeHelper<SubtitleAttribute>.GetAttributeValue(property);
            if (subTitleAttribute != null)
            {
                var sub = SubTitleTextBlock(subTitleAttribute.SubTitle);
                if (sub != null)
                {
                    stackPanel.Children.Add(sub);
                }
            }
            stackPanel.Children.Add(label);
            stackPanel.Children.Add(combo);
            stackPanel.Children.Add(txterror);
        }

        private void GenerateNumericUpDown(PropertyInfo property, PropertyInfo parentProperty = null)
        {
            var num = new NumericUpDown();
            var binding = new Binding();
            TextBlock txterror;
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
            binding.Source = this.DataContext;
            binding.Mode = BindingMode.TwoWay;
            // binding.NotifyOnValidationError = true;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            num.SetBinding(NumericUpDown.ValueProperty, binding);

            //var decimalCount = Helpers.AttributeHelper<DecimalCountAttribute>.GetAttributeValue(property);
            //if (decimalCount != null)
            //{
            //    num.nu = decimalCount.Number;
            //}

            var autoincrement = Helpers.AttributeHelper<AutoIncrementAttribute>.GetAttributeValue(property);
            if (autoincrement != null)
            {

                num.SmallChange = (double) autoincrement.Step;
            }
            var range = Helpers.AttributeHelper<RangeAttribute>.GetAttributeValue(property);
            if (range != null)
            {
                num.Minimum = range.Min;
                num.Maximum = range.Max;
            }

            var isEnabledAttribute = Helpers.AttributeHelper<IsEnabledPropertyAttribute>.GetAttributeValue(property);
            if (isEnabledAttribute != null)
            {
                if (!string.IsNullOrEmpty(isEnabledAttribute.PropertyToBind))
                {
                    Binding bindig3 = new Binding();
                    bindig3.Source = this.DataContext;

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
            var subTitleAttribute = Helpers.AttributeHelper<SubtitleAttribute>.GetAttributeValue(property);
            if (subTitleAttribute != null)
            {
                var sub = SubTitleTextBlock(subTitleAttribute.SubTitle);
                if (sub != null)
                {
                    stackPanel.Children.Add(sub);
                }
            }
            stackPanel.Children.Add(label);
            stackPanel.Children.Add(num);
            stackPanel.Children.Add(txterror);
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
            binding.Source = this.DataContext;
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
            var isEnabledAttribute = Helpers.AttributeHelper<IsEnabledPropertyAttribute>.GetAttributeValue(property);
            if (isEnabledAttribute != null)
            {
                if (!string.IsNullOrEmpty(isEnabledAttribute.PropertyToBind))
                {
                    Binding bindig3 = new Binding();
                    bindig3.Source = this.DataContext;

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
            var subTitleAttribute = Helpers.AttributeHelper<SubtitleAttribute>.GetAttributeValue(property);
            if (subTitleAttribute != null)
            {
                var sub = SubTitleTextBlock(subTitleAttribute.SubTitle);
                if (sub != null)
                {
                    stackPanel.Children.Add(sub);
                }
            }
            stackPanel.Children.Add(label);
            stackPanel.Children.Add(txt);
            stackPanel.Children.Add(txterror);
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
            binding.Source = this.DataContext;
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
                    bindig3.Source = this.DataContext;

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

            var subTitleAttribute = Helpers.AttributeHelper<SubtitleAttribute>.GetAttributeValue(property);
            if (subTitleAttribute != null)
            {
                var sub = SubTitleTextBlock(subTitleAttribute.SubTitle);
                if (sub != null)
                {
                    stackPanel.Children.Add(sub);
                }
            }

            stackPanel.Children.Add(label);
            stackPanel.Children.Add(picker);
            stackPanel.Children.Add(txterror);
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
            binding.Source = this.DataContext;
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
                    bindig3.Source = this.DataContext;

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
            var subTitleAttribute = Helpers.AttributeHelper<SubtitleAttribute>.GetAttributeValue(property);
            if (subTitleAttribute != null)
            {
                var sub = SubTitleTextBlock(subTitleAttribute.SubTitle);
                if (sub != null)
                {
                    stackPanel.Children.Add(sub);
                }
            }
            stackPanel.Children.Add(label);
            stackPanel.Children.Add(box);
            stackPanel.Children.Add(txterror);
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
            if (!string.IsNullOrEmpty(title) && stackPanel != null)
            {
                TextBlock txt = new TextBlock();
                txt.Text = title.ToUpper();
                txt.FontWeight = FontWeights.Bold;
                txt.FontSize = 18;
                txt.Margin = new Thickness(0, 5, 0, 5);
                stackPanel.Children.Insert(0, txt);
            }
        }

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

        private static  void ValidationCollectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
            var textBlocks = this.GetDescendantsOfType<TextBlock>();
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
                        var element = stackPanel.FindName(nameToFind);
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