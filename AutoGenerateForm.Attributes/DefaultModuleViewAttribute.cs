using System;

namespace AutoGenerateForm.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class)]

   
    public class DefaultModuleViewAttribute : Attribute
    {
        public bool IsDefault { get; set; }
        /// <summary>
        /// Indicate if the framework will use the default view or custom view
        /// </summary>
        /// <param name="isDefault"></param>
        public DefaultModuleViewAttribute(bool isDefault = true)
        {
            IsDefault = isDefault;
        }
    }
}