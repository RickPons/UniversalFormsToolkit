using System.Linq;
using System.Reflection;
namespace AutoGenerateForm.Helpers
{
    public static class AttributeHelper<T>
    {
        public static T GetAttributeValue(PropertyInfo itemToExtractAttribute)
        {

            var attribute = itemToExtractAttribute.GetCustomAttributes(typeof(T), false).Cast<T>().SingleOrDefault();

            return attribute;
        }

       
    }
}