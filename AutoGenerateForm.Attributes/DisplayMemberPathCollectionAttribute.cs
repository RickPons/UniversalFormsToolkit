
namespace AutoGenerateForm.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property |
                           System.AttributeTargets.Struct)]
    public class DisplayMemberPathCollectionAttribute: System.Attribute
    {
        public string DisplayMemberPath { get; set; }
        public DisplayMemberPathCollectionAttribute( string displayMemberPath="")
        {
            DisplayMemberPath = displayMemberPath;
        }
    }
}
