using AutoGenerateForm.Attributes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UniversalFormsToolkit.ValidationExample.Models;
using System;

namespace UniversalFormsToolkit.ValidationExample.ViewModels
{
    public class MainPageViewModel: PropertyChangeBase
    {

        public RelayCommand HideCommand { get; set; }


        private Models.Group myGroup;

        public Models.Group MyGroup
        {
            get { return myGroup; }
            set
            {
                myGroup = value;
                NotifyPropertyChanged();
            }
        }

        private Student selectedStudent;
        [AutoGenerateProperty]
        public Student SelectedStudent
        {
            get { return selectedStudent; }
            set
            {
                selectedStudent = value;
                NotifyPropertyChanged();
            }
        }

        public MainPageViewModel()
        {
            HideCommand = new RelayCommand(HideCommandExecute);
            var list = new List<Course>()
            {
                 new Course
                 {
                      Name= "Programming",

                 },
                 new Course
                 {
                     Name="Design"
                 }
            };

            MyGroup = new Models.Group();
            MyGroup.Students = new ObservableCollection<Student>()
            {
                new Student
                {
                     Age=20,
                      Name="Ricardo",
                       BirthDay= null,
                       LastName="Pons",
                        Courses= new ObservableCollection<Course>(list)
                },
                new Student
                {
                     Age=22,
                      Name="Bruno",
                       LastName="Medina",
                       Courses= new ObservableCollection<Course>(list)
                },
                new Student
                {
                     Age=21,
                      LastName="Ronald",
                       Name="Becker",
                       Courses= new ObservableCollection<Course>(list)
                }
            };

            
            
        }

        private void HideCommandExecute()
        {
            if (SelectedStudent != null)
            {
                SelectedStudent.HideBirthDay = SelectedStudent.HideBirthDay ? false : true;
            }
        }
    }
}
