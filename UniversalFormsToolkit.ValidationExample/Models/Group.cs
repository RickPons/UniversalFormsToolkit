using AutoGenerateForm.Attributes;
using System.Collections.ObjectModel;

namespace UniversalFormsToolkit.ValidationExample.Models
{
    public class Group : PropertyChangeBase
    {

        private ObservableCollection<Student> students;

        public ObservableCollection<Student> Students
        {
            get
            {
                return students;
            }
            set
            {
                students = value;
                NotifyPropertyChanged();
            }
        }

        

    }
}
