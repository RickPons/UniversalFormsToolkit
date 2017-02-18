using AutoGenerateForm.Attributes;
using MvvmValidation;
using System.Collections.ObjectModel;
using System;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AutoGenerateForm.Helpers;

namespace UniversalFormsToolkit.ValidationExample.Models
{
    public class Student : PropertyChangeBase, IValidatable, INotifyDataErrorInfo
    {


        private bool hideBirthday;

        public bool HideBirthDay
        {
            get { return hideBirthday; }
            set
            {
                hideBirthday = value;
                NotifyPropertyChanged();
            }
        }

        private string name;

        [AutoGenerateProperty]
        [Display("Name")]
        [IsVisible("HideBirthDay")]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged();
                Validator.Validate(() => Name);
            }
        }
        private string lastName;
        [AutoGenerateProperty]
        [Display("Last Name")]
        [IsVisible("HideBirthDay")]
        [IsEnabledProperty(false)]
        public string LastName
        {
            get { return lastName; }
            set
            {
                lastName = value;
                NotifyPropertyChanged();
                Validator.Validate(() => Name);
            }
        }
        private int age;
        [IsNumeric]
        [Range(0,1000)]
        [DecimalCount(2)]
        [AutoIncrement(2)]
        [IsVisible("HideBirthDay")]
        [AutoGenerateProperty]
        [Display("Age")]
     
        public int Age
        {
            get { return age; }
            set
            {
                age = value;
                NotifyPropertyChanged();
            }
        }

        private string phoneNumber;
        [AutoGenerateProperty]
        [Display("Phone Number")]
        [StringLength(4)]
        [IsVisible("HideBirthDay")]
        [Subtitle("My Subtitle")]
      
        public string PhoneNumber
        {
            get
            {
                return phoneNumber;
            }
            set
            {
                phoneNumber = value;
                NotifyPropertyChanged();
            }
        }


        private ObservableCollection<Course> courses;
        [AutoGenerateProperty]
        [DisplayMemberPathCollection("Name")]
        [SelectedItemCollection("SelectedCourse")]
        [IsVisible("HideBirthDay")]

        public ObservableCollection<Course> Courses
        {
            get { return courses; }
            set
            {
                courses = value;
                NotifyPropertyChanged();
            }
        }


        private Course selectedCourse;

        [Display("Courses")]
        public Course SelectedCourse
        {
            get { return selectedCourse; }
            set
            {
                selectedCourse = value;
                NotifyPropertyChanged();
            }
        }

        private DateTime? birthDay;

        [AutoGenerateProperty]
        [Display("Birthday")]

      
        public DateTime? BirthDay
        {
            get { return birthDay; }
            set
            {
                birthDay = value;
                NotifyPropertyChanged();
            }
        }

        private bool isGraduated;
        [AutoGenerateProperty]
        [Display("Is Graduated")]
     
        public bool IsGraduated
        {
            get { return isGraduated; }
            set
            {
                isGraduated = value;
                NotifyPropertyChanged();
            }
        }

        #region Validations

        private ObservableCollection<AutoGenerateForm.Uwp.Models.ValidationModel> validations;

        public ObservableCollection<AutoGenerateForm.Uwp.Models.ValidationModel> Validations
        {
            get { return validations; }
            set
            {
                if (validations == value)
                    return;
                validations = value;
                NotifyPropertyChanged();
            }
        }

      

        public Student()
        {
        
            Validator = new ValidationHelper();

            this.PropertyChanged += Student_PropertyChanged;
            NotifyDataErrorInfoAdapter = new NotifyDataErrorInfoAdapter(Validator);


            OnCreated();
            ConfigureRules();
        }

        private async void Student_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Validations")
                return;
            var result = await Validator.ValidateAllAsync();
            if (result != null && result.IsValid)
            {
                Validations = new ObservableCollection<AutoGenerateForm.Uwp.Models.ValidationModel>();
                return;
            }
            var errors = new List<AutoGenerateForm.Uwp.Models.ValidationModel>();
            if (result != null && result.ErrorList!=null && !result.IsValid)
            {
                foreach (var item in result.ErrorList)
                {
                    var error = new AutoGenerateForm.Uwp.Models.ValidationModel();
                    var property = this.GetType().GetProperty(item.Target.ToString());

                    if (property != null)
                    {
                        var displayAttribute = AttributeHelper<DisplayAttribute>.GetAttributeValue(property);
                        error.Label = displayAttribute?.Label;
                    }
                    error.ErrorMessage = item.ErrorText;
                    error.ParentPropertyName = "";
                    error.PropertyName = item.Target.ToString();
                    if (item.Target.Equals("SelectedCourse"))
                    {
                        error.PropertyName = "Courses";
                        error.ParentPropertyName = "";
                    }
                   
                    errors.Add(error);
                }
            }
            this.Validations = new ObservableCollection<AutoGenerateForm.Uwp.Models.ValidationModel>(errors);
        }

        private void ConfigureRules()
        {
            Validator.AddRequiredRule(() => Name, "Name is required field");
            Validator.AddRequiredRule(() => LastName, "Last Name is required field");
            validator.AddRequiredRule(() => SelectedCourse, "Please select a course");
        }

        void OnCreated()
        {
        }

        Task<ValidationResult> IValidatable.Validate()
        {
            return Validator.ValidateAllAsync();
        }

      

        public IEnumerable GetErrors(string propertyName)
        {
            return NotifyDataErrorInfoAdapter.GetErrors(propertyName);
        }

        public bool HasErrors
        {
            get
            {
                return NotifyDataErrorInfoAdapter.HasErrors;
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add
            {
                NotifyDataErrorInfoAdapter.ErrorsChanged += value;
            }
            remove
            {
                NotifyDataErrorInfoAdapter.ErrorsChanged -= value;
            }
        }

        #endregion

        #region EditableEntity Interface implementation

    

        private NotifyDataErrorInfoAdapter notifyDataErrorInfoAdapter;

        private ValidationHelper validator;

        public ValidationHelper Validator
        {
            get
            {
                return validator;
            }
            set
            {
                validator = value;
            }
        }

        public NotifyDataErrorInfoAdapter NotifyDataErrorInfoAdapter
        {
            get
            {
                return notifyDataErrorInfoAdapter;
            }
            set
            {
                notifyDataErrorInfoAdapter = value;
            }
        }

     
      

        
       
        #endregion

    }
}
