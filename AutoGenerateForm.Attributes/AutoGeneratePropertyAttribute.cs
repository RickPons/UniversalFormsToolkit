using System;

namespace AutoGenerateForm.Attributes
{
     [System.AttributeUsage(System.AttributeTargets.Property |
                           System.AttributeTargets.Struct)]
    public class AutoGeneratePropertyAttribute:Attribute
    {
        public bool AutoGenerate { get; set; }

        public AutoGeneratePropertyAttribute(bool autoGenerate=true)
        {
            this.AutoGenerate = autoGenerate;
        }
    }
}
