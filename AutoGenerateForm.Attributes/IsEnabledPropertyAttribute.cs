using System;

namespace AutoGenerateForm.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property |
                           System.AttributeTargets.Struct)]
    public class IsEnabledPropertyAttribute : Attribute
    {
        public bool IsEnabled { get; private set; }

        public string PropertyToBind { get; private set; }

        public IsEnabledPropertyAttribute(bool isEnabled=true)
        {
            this.IsEnabled = IsEnabled;
        }

        public IsEnabledPropertyAttribute(string propertyTobind)
        {
            this.PropertyToBind = propertyTobind;
        }
    }
}