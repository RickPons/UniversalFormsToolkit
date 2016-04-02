using AutoGenerateForm.Attributes;
using System.Collections.ObjectModel;

namespace UniversalFormsToolkit.CollectionsExample.Models
{
    public class Group:PropertyChangeBase
    {
        private ObservableCollection<Student> students;

        [AutoGenerateProperty]
        [DisplayMemberPathCollection("Name")]
        [SelectedItemCollection("SelectedStudent")]
        [Display("Students")]
        [PropertyOrder(2)]
        public ObservableCollection<Student> Students
        {
            get { return students; }
            set
            {
                students = value;
                NotifyPropertyChanged();
            }
        }

        private Student selectedStudent;

        public Student SelectedStudent
        {
            get { return selectedStudent; }
            set
            {
                selectedStudent = value;
                NotifyPropertyChanged();
            }
        }


        private string groupName;

        [AutoGenerateProperty]
        [Display("Group name")]
        [PropertyOrder(1)]

        public string GroupName
        {
            get { return groupName; }
            set
            {
                groupName = value;
                NotifyPropertyChanged();
            }
        }


    }
}
