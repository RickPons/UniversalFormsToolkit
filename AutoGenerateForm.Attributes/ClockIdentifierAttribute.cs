using System;

namespace AutoGenerateForm.Attributes
{
    public class ClockIdentifierAttribute:Attribute
    {
        public string ClockFormat { get; set; }

        
        public ClockIdentifierAttribute(bool is24HourClock=false)
        {
            if ((is24HourClock))
            {
                ClockFormat = "24HourClock";
            }
            else
            {
                ClockFormat = "12HourClock";
            }
        }
    }
}
