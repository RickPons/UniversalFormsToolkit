using System;

namespace AutoGenerateForm.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property |
                           System.AttributeTargets.Struct)]
    public class RangeAttribute : Attribute
    {
       
        public int Min { get; set; }

        public int Max { get; set; }

        public RangeAttribute(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }

    }
}