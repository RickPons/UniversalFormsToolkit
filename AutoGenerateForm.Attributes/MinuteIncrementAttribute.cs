using System;

namespace AutoGenerateForm.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class MinuteIncrementAttribute:Attribute
    {
        public int Number { get; set; }

        /// <summary>
        /// sets a value that indicates the time increments shown in the minute picker. For example, 15 specifies that the TimePicker minute control displays only the choices 00, 15, 30, 45.

            /// https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.controls.timepicker    
        /// </summary>
        /// <param name="step"></param>
        public MinuteIncrementAttribute(int step)
        {
            if (step<=0)
            {
                step = 1;
               
            }

            if (step>59)
            {
                step = 59;
               
            }
            Number = step;
        }
    }
}
