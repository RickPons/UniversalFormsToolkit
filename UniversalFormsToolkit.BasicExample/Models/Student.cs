using AutoGenerateForm.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalFormsToolkit.BasicExample.Models
{
    public class Student
    {
        //If we don't want the property to be in 
        //the form we just don't add an annotation
        private int ID { get;  set; }

        [AutoGenerateProperty]
        [Display("First Name")]//These two anotations create one text box called "First Name"
        public string FirstName { get; set; }

        [AutoGenerateProperty]
        [Display("Last Name")]
        public string LastName { get; set; }

        [AutoGenerateProperty]
        [Display("Birthday")]
        
        public DateTime Birthday { get; set; }

        [AutoGenerateProperty]
        [Display("Semester")]
        public int Semester { get; set; }
    }
}
