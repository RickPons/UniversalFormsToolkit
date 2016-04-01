
namespace AutoGenerateForm.Attributes
{
     [System.AttributeUsage(System.AttributeTargets.Property |
                           System.AttributeTargets.Struct)]
    public class PropertyOrderAttribute: System.Attribute
    {
        public int Order { get; set; }
        public PropertyOrderAttribute( int order)
        {
            Order = order;
        }
    }
}
