using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace AutoGenerateForm.Uwp
{
    public sealed partial class ValidationSummary : UserControl
    {
        public ValidationSummary()
        {
            this.InitializeComponent();
        }

       
        private void listView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (AutoGenerator == null)
                return;
            if (e.ClickedItem != null && e.ClickedItem is AutoGenerateForm.Uwp.Models.ValidationModel)
            {
                var validationModel = e.ClickedItem as AutoGenerateForm.Uwp.Models.ValidationModel;


                var behaviors = Microsoft.Xaml.Interactivity.Interaction.GetBehaviors(AutoGenerator);
                var item = behaviors.Where(x => x.GetType() == typeof(Behaviors.SetFocusFromPropertyBehavior)).FirstOrDefault();
                if (item != null)
                {
                    var behavior = item as Behaviors.SetFocusFromPropertyBehavior;
                    if (behavior != null)
                    {
                        behavior.ParentPropertyToSetFocus = validationModel.ParentPropertyName;
                        behavior.PropertyToSetFocus = validationModel.PropertyName;


                    }
                }

            }
        }

        public AutoGenerateForm.Uwp.AutoGenerator AutoGenerator
        {
            get { return (AutoGenerateForm.Uwp.AutoGenerator) GetValue(AutoGeneratorProperty); }
            set { SetValue(AutoGeneratorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoGenerator.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoGeneratorProperty =
            DependencyProperty.Register("AutoGenerator", typeof(AutoGenerateForm.Uwp.AutoGenerator), typeof(ValidationSummary), new PropertyMetadata(null, AutoGeneratorPropertyChanged));

        private static void AutoGeneratorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ValidationSummary;
            if (control == null || e.NewValue == null)
                return;

            if (e.NewValue is AutoGenerateForm.Uwp.AutoGenerator)
            {
                var behavior = new Behaviors.SetFocusFromPropertyBehavior();
                var behaviors = Microsoft.Xaml.Interactivity.Interaction.GetBehaviors((DependencyObject) e.NewValue);
                if (!behaviors.Any(x => x.GetType() == typeof(Behaviors.SetFocusFromPropertyBehavior)))
                {
                    Microsoft.Xaml.Interactivity.Interaction.GetBehaviors((DependencyObject) e.NewValue).Add(behavior);
                }
            }

        }

        public ObservableCollection<AutoGenerateForm.Uwp.Models.ValidationModel> ValidationCollection
        {
            get { return (ObservableCollection<AutoGenerateForm.Uwp.Models.ValidationModel>) GetValue(ValidationCollectionProperty); }
            set { SetValue(ValidationCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValidationCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValidationCollectionProperty =
            DependencyProperty.Register("ValidationCollection", typeof(ObservableCollection<AutoGenerateForm.Uwp.Models.ValidationModel>), typeof(ValidationSummary), new PropertyMetadata(null));


    }
}
