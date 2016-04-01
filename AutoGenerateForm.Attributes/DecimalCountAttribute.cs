
namespace AutoGenerateForm.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property |
                           System.AttributeTargets.Struct)]
    public class DecimalCountAttribute: System.Attribute
    {
        public int Number { get; set; }
        public DecimalCountAttribute( int number)
        {
            Number = number;
        }
    }
}
