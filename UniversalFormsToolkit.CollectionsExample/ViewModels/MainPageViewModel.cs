using AutoGenerateForm.Attributes;
using UniversalFormsToolkit.CollectionsExample.Models;

namespace UniversalFormsToolkit.CollectionsExample.ViewModels
{
    public class MainPageViewModel : PropertyChangeBase
    {

        public MainPageViewModel()
        {
            this.MyGroup = new Group();
            this.MyGroup.GroupName = "My group";
            this.MyGroup.Students = new System.Collections.ObjectModel.ObservableCollection<Student>()

            {
                new Student() { Name="Ricardo", LastName="Pons", Age=25 },
                new Student() { Name = "Bruno", LastName = "Medina", Age = 19 },
                new Student() { Name = "Ronald", LastName = "Becker", Age = 40 }
            };
        }

      

        private Group myGroup;

        [AutoGenerateProperty]
        public Group MyGroup
        {
            get { return myGroup; }
            set
            {
                myGroup = value;
                NotifyPropertyChanged();
            }
        }

    }
}
