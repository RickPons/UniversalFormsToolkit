using System;

namespace AutoGenerateForm.Attributes
{
    public class StringLengthAttribute:Attribute
    {
        public int Count { get; set; }

        public StringLengthAttribute(int count)
        {
            Count = count;
        }
    }
}
