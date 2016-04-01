
namespace AutoGenerateForm.Attributes
{

    [System.AttributeUsage(System.AttributeTargets.Property |
                           System.AttributeTargets.Struct)]
    public class DisplayAttribute: System.Attribute
    {
        public string Label { get; set; }
        public DisplayAttribute( string label="")
        {
            Label = label;
        }
    }
}
