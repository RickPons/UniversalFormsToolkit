using System;

namespace AutoGenerateForm.Attributes
{
    public class SubtitleAttribute:Attribute
    {
        public string SubTitle { get; set; }

        public SubtitleAttribute(string subtitle)
        {
            this.SubTitle = subtitle;
        }
    }
}
