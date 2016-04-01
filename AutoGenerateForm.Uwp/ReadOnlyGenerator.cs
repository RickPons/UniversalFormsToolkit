using AutoGenerateForm.Attributes;
using AutoGenerateForm.Uwp.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace AutoGenerateForm.Uwp
{
    public class ReadOnlyGenerator : UserControl
    {

        ContentControl control;
        StackPanel stack;
        Grid grid;
        ProgressRing ring;
        bool IsViewReady;
        public bool Load
        {
            get { return (bool) GetValue(LoadProperty); }
            set { SetValue(LoadProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Load.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadProperty =
            DependencyProperty.Register("Load", typeof(bool), typeof(ReadOnlyGenerator), new PropertyMetadata(false, LoadPropertyChanged));

        private static async void LoadPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ReadOnlyGenerator;
            if (control == null)
                return;
            if ((bool) e.NewValue)
            {
                await Task.Delay(500);
                await control.GenerateReadOnlyFormAsync();
            }

        }

        public object CurrentContext
        {
            get { return (object) GetValue(CurrentContextProperty); }
            set { SetValue(CurrentContextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentContext.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentContextProperty =
            DependencyProperty.Register("CurrentContext", typeof(object), typeof(ReadOnlyGenerator), new PropertyMetadata(null));


        private async Task GenerateReadOnlyFormAsync()
        {
            IsViewReady = false;
            control = new ContentControl();
            var collection = new TransitionCollection();
            var theme = new EntranceThemeTransition();

            //var info = new ContinuumNavigationTransitionInfo();

            //theme.DefaultNavigationTransitionInfo = info;
            collection.Add(theme);



            control.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            control.VerticalContentAlignment = VerticalAlignment.Stretch;
            grid = new Grid();
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.Transitions = collection;
            ring = new ProgressRing();
            stack = new StackPanel();
            stack.HorizontalAlignment = HorizontalAlignment.Stretch;
            stack.VerticalAlignment = VerticalAlignment.Stretch;
            ring.HorizontalAlignment = HorizontalAlignment.Center;
            ring.VerticalAlignment = VerticalAlignment.Center;
            ring.Height = 75;
            ring.Width = 75;
            ring.Foreground = new SolidColorBrush(Colors.White);

            grid.Children.Add(ring);
            control.Content = grid;
            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            this.VerticalContentAlignment = VerticalAlignment.Stretch;
            this.Content = control;



            ring.IsActive = true;

            ring.Foreground = this.ForegroundText;

            if (this.CurrentContext == null)
            {
                ring.IsActive = false;
                var textBlock = new TextBlock();
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.FontSize = 16;
                textBlock.FontWeight = FontWeights.Light;
                textBlock.Foreground = new SolidColorBrush(Colors.White);
                textBlock.Text = "No hay datos para mostrar";
                this.Content = textBlock;
            }
            else
            {
                Type mainType = this.CurrentContext.GetType();

                var props = new List<PropertyInfo>(mainType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                                          .Where(x => x.GetCustomAttributes(typeof(AutoGeneratePropertyAttribute), false).Any() &&
                                                                                      x.GetCustomAttributes(typeof(AutoGeneratePropertyAttribute), false)
                                                                                       .Cast<AutoGeneratePropertyAttribute>()
                                                                                       .Any(z => z.AutoGenerate == true))
                                                                          .ToList());
                var orderedItems = await GetOrderedProperties(props);

                await GenerateForm(orderedItems);
            }
        }
        private Task<List<PropertyInfo>> GetOrderedProperties(List<PropertyInfo> properties)
        {
            return Task.Factory.StartNew<List<PropertyInfo>>(() =>
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

             });

        }

        private async Task GenerateForm(List<PropertyInfo> orderedprops, PropertyInfo parentProperty = null)
        {
            await Task.Factory.StartNew(async () =>
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
                           var props = new List<PropertyInfo>(propertyType
                                                                                          .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                                                          .Where(x => x.GetCustomAttributes(typeof(AutoGeneratePropertyAttribute), false).Any() &&
                                                                                                      x.GetCustomAttributes(typeof(AutoGeneratePropertyAttribute), false)
                                                                                                       .Cast<AutoGeneratePropertyAttribute>()
                                                                                                       .Any(z => z.AutoGenerate == true))
                                                                                          .ToList());

                           var orderedprops1 = await GetOrderedProperties(props);
                           await GenerateForm(orderedprops1, property);
                       }
                       else
                       {
                           await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                GenerateControls(property, parentProperty);
                            });

                       }
                   }
                   await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                   {
                       if (!IsViewReady)
                       {
                           IsViewReady = true;
                           await Task.Delay(2000);
                           ring.IsActive = false;

                           //await Task.Delay(500);
                           grid.Children.Add(stack);


                       }



                   });

               }
           });
        }


        public SolidColorBrush ForegroundText
        {
            get { return (SolidColorBrush) GetValue(ForegroundTextProperty); }
            set { SetValue(ForegroundTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForegroundText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForegroundTextProperty =
            DependencyProperty.Register("ForegroundText", typeof(SolidColorBrush), typeof(ReadOnlyGenerator), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public double LabelFontSize
        {
            get { return (double) GetValue(SubTitleFonSizeProperty); }
            set { SetValue(SubTitleFonSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubTitleFonSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubTitleFonSizeProperty =
            DependencyProperty.Register("LabelFontSize", typeof(double), typeof(ReadOnlyGenerator), new PropertyMetadata((double) 14));



        public FontWeight LabelFontWeight
        {
            get { return (FontWeight) GetValue(SubTitlefontWeightProperty); }
            set { SetValue(SubTitlefontWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubTitlefontWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubTitlefontWeightProperty =
            DependencyProperty.Register("LabelFontWeight", typeof(FontWeight), typeof(ReadOnlyGenerator), new PropertyMetadata(FontWeights.SemiLight));



        public FontWeight PropertyTextWeight
        {
            get { return (FontWeight) GetValue(PropertyTextWeightProperty); }
            set { SetValue(PropertyTextWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PropertyTextWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyTextWeightProperty =
            DependencyProperty.Register("PropertyTextWeight", typeof(FontWeight), typeof(ReadOnlyGenerator), new PropertyMetadata(FontWeights.Light));


        public double PropertyTextFontSize
        {
            get { return (double) GetValue(PropertyTextFontSizeProperty); }
            set { SetValue(PropertyTextFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PropertyTextFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyTextFontSizeProperty =
            DependencyProperty.Register("PropertyTextFontSize", typeof(double), typeof(ReadOnlyGenerator), new PropertyMetadata((double) 11));





        private async void GenerateControls(PropertyInfo property, PropertyInfo parentProperty)
        {
            var label = GenerateLabelPropertyName(property);
            var propertyType = property.PropertyType;
            var tColl = typeof(ICollection<>);
            if (propertyType.GetTypeInfo().IsGenericType && tColl.IsAssignableFrom(propertyType.GetGenericTypeDefinition()) ||
                propertyType.GetInterfaces().Any(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == tColl))


            {


                var selectedItemAttribute = Helpers.AttributeHelper<SelectedItemCollectionAttribute>.GetAttributeValue(property);
                if (selectedItemAttribute != null)
                {
                    var binding2 = new Windows.UI.Xaml.Data.Binding();
                    var memberPath = string.Empty;
                    var displayMemberPathAttribute = Helpers.AttributeHelper<DisplayMemberPathCollectionAttribute>.GetAttributeValue(property);
                    if (displayMemberPathAttribute != null)
                    {
                        memberPath = displayMemberPathAttribute.DisplayMemberPath;
                    }

                    if (!string.IsNullOrEmpty(memberPath))
                    {
                        memberPath = "." + memberPath;
                    }
                    if (parentProperty != null)
                    {
                        binding2.Path = new PropertyPath((parentProperty.Name + "." + selectedItemAttribute.PropertyNameToBind + memberPath).Trim());
                    }
                    else
                    {
                        binding2.Path = new PropertyPath((selectedItemAttribute.PropertyNameToBind + memberPath).Trim());
                    }
                    binding2.Source = this.DataContext;
                    binding2.Mode = BindingMode.TwoWay;
                    binding2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    //  binding2.NotifyOnValidationError = true;

                    var num = new TextBlock();
                    num.Margin = new Thickness(0, 8, 0, 8);
                    num.FontSize = this.PropertyTextFontSize;
                    num.FontWeight = this.PropertyTextWeight;
                    var foregroundBinding = new Binding();
                    foregroundBinding.Source = this;
                    foregroundBinding.Mode = BindingMode.TwoWay;
                    foregroundBinding.Path = new PropertyPath("ForegroundText");
                    num.SetBinding(TextBlock.ForegroundProperty, foregroundBinding);
                    num.SetBinding(TextBlock.TextProperty, binding2);
                    stack.Children.Add(label);
                    stack.Children.Add(num);
                }
            }
            else
            {


                if (propertyType.Equals(typeof(bool)) || propertyType.Equals(typeof(Nullable<bool>)))
                {
                    var text = GeneratePropertyBinding(property, parentProperty,true);
                    stack.Children.Add(label);
                    stack.Children.Add(text);
                }
                else
                {
                    var text = GeneratePropertyBinding(property, parentProperty);
                    stack.Children.Add(label);
                    stack.Children.Add(text);
                }
              
            }


            //var propertyType = property.PropertyType;
            //if (propertyType.Equals(typeof(int)) ||
            //    propertyType.Equals(typeof(long)) ||
            //    propertyType.Equals(typeof(Nullable<int>)) ||
            //    propertyType.Equals(typeof(Nullable<long>)))
            //{


            //    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //    {
            //        var isNumeric = Helpers.AttributeHelper<IsNumericAttribute>.GetAttributeValue(property);
            //        if (isNumeric != null)
            //        {
            //            GenerateNumericUpDown(property, parentProperty);
            //        }
            //        else
            //        {


            //            GenerateTextBox(property, parentProperty);
            //        }

            //    });
            //    return;
            //}

            //if (propertyType.Equals(typeof(string)))
            //{
            //    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //    {
            //        var isSuggestionsEnabled = Helpers.AttributeHelper<IsSuggestionsEnabledAttribute>.GetAttributeValue(property);
            //        if (isSuggestionsEnabled != null)
            //        {
            //            GenerateSuggestionsControl(property, parentProperty);
            //        }
            //        else
            //        {

            //            GenerateTextBox(property, parentProperty);
            //        }
            //    });
            //    return;
            //}

            //if (propertyType.Equals(typeof(float)) ||
            //    propertyType.Equals(typeof(decimal)) ||
            //    propertyType.Equals(typeof(double)) ||
            //    propertyType.Equals(typeof(Nullable<double>)) ||
            //    propertyType.Equals(typeof(Nullable<decimal>)) ||
            //    propertyType.Equals(typeof(Nullable<float>))

            //)
            //{

            //    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //    {
            //        GenerateNumericUpDown(property, parentProperty);
            //    });


            //    return;
            //}


            //if (propertyType.Equals(typeof(DateTime)) || propertyType.Equals(typeof(Nullable<DateTime>)))
            //{

            //    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //    {
            //        GenerateDatePicker(property, parentProperty);
            //    });
            //    return;
            //}

            //if (propertyType.Equals(typeof(bool)) || propertyType.Equals(typeof(Nullable<bool>)))
            //{


            //    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //    {
            //        GenerateCheckBox(property, parentProperty);
            //    });
            //    return;
            //}

            //if (propertyType.Equals(typeof(TimeSpan)) || propertyType.Equals(typeof(Nullable<TimeSpan>)))
            //{

            //    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //    {
            //        GenerateDateTimePicker(property, parentProperty);
            //    });


            //    return;
            //}
        }



        private TextBlock GeneratePropertyBinding(PropertyInfo property, PropertyInfo parentProperty, bool isBool = false)
        {
            var num = new TextBlock();
            num.FontSize = this.PropertyTextFontSize;
            var foregroundBinding = new Binding();
            foregroundBinding.Source = this;
            foregroundBinding.Mode = BindingMode.TwoWay;
            foregroundBinding.Path = new PropertyPath( "ForegroundText");
            num.SetBinding(TextBlock.ForegroundProperty, foregroundBinding);
            num.Foreground = this.ForegroundText;
            num.FontWeight = this.PropertyTextWeight;
            num.Margin = new Thickness(0, 8, 0, 8);
            var binding = new Binding();
            if (parentProperty != null)
            {
                binding.Path = new PropertyPath(parentProperty.Name + "." + property.Name);
                num.Name = parentProperty.Name + "_" + property.Name;
            }
            else
            {
                binding.Path = new PropertyPath(property.Name);
                num.Name = property.Name;
            }
            if (isBool)
            {
                binding.Converter = new BoolToYesNoConverter();
            }
            binding.Source = this.DataContext;
            binding.Mode = BindingMode.TwoWay;
            // binding.NotifyOnValidationError = true;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            num.SetBinding(TextBlock.TextProperty, binding);

            return num;
        }
        private TextBlock GenerateLabelPropertyName(PropertyInfo property)
        {
            try
            {
                var label = new TextBlock();
                var foregroundBinding = new Binding();
                foregroundBinding.Source = this;
                foregroundBinding.Mode = BindingMode.TwoWay;
                foregroundBinding.Path = new PropertyPath("ForegroundText");
                label.SetBinding(TextBlock.ForegroundProperty, foregroundBinding);
                label.FontSize = this.LabelFontSize;
                label.FontWeight = this.LabelFontWeight;
                var displayAttribute = Helpers.AttributeHelper<DisplayAttribute>.GetAttributeValue(property);
                if (displayAttribute != null)
                {
                    label.Text = displayAttribute.Label;
                }
                else
                {
                    label.Text = property.Name;
                }

                return label;
            }
            catch (Exception ex)
            {


            }
            return null;
            //var binding = new Binding();
            //if (parentProperty != null)
            //{
            //    binding.Path = new PropertyPath(parentProperty.Name + "." + property.Name);
            //    num.Name = parentProperty.Name + "_" + property.Name;
            //}
            //else
            //{
            //    binding.Path = new PropertyPath(property.Name);
            //    num.Name = property.Name;
            //}
            //binding.Source = this.DataContext;
            //binding.Mode = BindingMode.TwoWay;
            //// binding.NotifyOnValidationError = true;
            //binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //num.SetBinding(NumericUpDown.ValueProperty, binding);
        }

    }
}
