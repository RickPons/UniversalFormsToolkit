using System;

namespace AutoGenerateForm.Attributes
{
    public class SelectedItemCollectionAttribute : Attribute
    {

        public string PropertyNameToBind { get; set; }

        public SelectedItemCollectionAttribute(string propertyNameToBind)
        {
            this.PropertyNameToBind = propertyNameToBind;
        }
    }
}
