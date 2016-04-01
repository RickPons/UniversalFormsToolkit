using System;

namespace AutoGenerateForm.Attributes
{

    [System.AttributeUsage(System.AttributeTargets.Property |
                         System.AttributeTargets.Struct)]
    public class IsSuggestionsEnabledAttribute:Attribute
    {

        public string CollectionName { get; set; }
        public string CollectionBindingDisplayName { get; set; }

        public IsSuggestionsEnabledAttribute(string collectionName, string collectionBindingDisplayName)
        {
            CollectionName = collectionName;
            CollectionBindingDisplayName = collectionBindingDisplayName;
        }

    }
}
