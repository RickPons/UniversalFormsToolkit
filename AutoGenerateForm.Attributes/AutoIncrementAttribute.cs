using System;

namespace AutoGenerateForm.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property |
                        System.AttributeTargets.Struct)]
    public class AutoIncrementAttribute : Attribute
    {
        public double Step { get; set; }

        public AutoIncrementAttribute(double step = 1)
        {
            this.Step = step;
        }
    }
}
