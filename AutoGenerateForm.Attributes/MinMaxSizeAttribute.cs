using System;

namespace AutoGenerateForm.Attributes
{
     [System.AttributeUsage(System.AttributeTargets.Property |
                           System.AttributeTargets.Struct)]
    public class MinMaxSizeAttribute : Attribute
    {
        public int MinWidth { get; set; }

        public int MaxWidth { get; set; }

        public int MinHeight { get; set; }
        public int MaxHeight { get; set; }

      /// <summary>
      ///   /// Attribute to set min and max size of some control 
         /// Just works with TextBox by now
      /// </summary>
      /// <param name="minWidth">0 is Auto</param>
      /// <param name="maxWidth">0 is Auto</param>
      /// <param name="minHeight">0 is Auto</param>
      /// <param name="maxHeight">0 is Auto</param>
        public MinMaxSizeAttribute(int minWidth, int maxWidth,int minHeight,int maxHeight)
        {
            this.MinHeight = minHeight;
            this.MinWidth = minWidth;
            this.MaxHeight = maxHeight;
            this.MinHeight = minHeight;
           
        }
    }
}