namespace AutoGenerateForm.Uwp.Models
{
    public class ValidationModel
    {
        public string PropertyName { get; set; }

        public string Label { get; set; }

        public string ErrorMessage { get; set; }
        public string ParentPropertyName { get; set; }
    }
}
