using System;

namespace AutoGenerateForm.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property |
                          System.AttributeTargets.Struct)]
    public class IsVisibleAttribute:Attribute
    {
     
        public string PropertyNameToBind { get; set; }
   

        public IsVisibleAttribute( string propertyNameToBind)
        {
            this.PropertyNameToBind = propertyNameToBind;
        }
    }
}
