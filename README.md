# UniversalFormsToolkit

This small UWP framework for Windows 10 builds input forms that can be used in your views based only on you classes defined in your model's folder. 


## Features: 

- Create an input form from a business object defined in your models.
- You can set the order of the controls using attributes.
- This framework creates common controls like text boxes, combo boxes, DateTimePicker, NumericUpdown and many more.
- Make your forms read-only.
- Add simple validations to your form.

See also:

- UniversalFormsToolkit works better with validations through MVVM Validation Helpers.
- You can add  your own logic to validate your bussines objects.

## Nuget:

Available in Nuget

https://www.nuget.org/packages/UniversalFormsToolkit/

    Install-Package UniversalFormsToolkit 


## Example:

#### 1 - Lets start with a basic class *Student*

```csharp
public class Student 
{
    private int ID { get; private set; }
    public string FirstName { get; set; }
    public string LastName{ get; set; }
    public DateTime Birthday{ get; set; }
    public int Semester{ get; set; }
}
```        

#### 2 - After we have installed the nuget package, we must use the AutoGenerateForm.Uwp namespace and finally, we must add "annotations" to the properties of our class that we want to display in our form.

```csharp
using AutoGenerateForm.Uwp;
public class Student 
{
    //If we don't want the property to be in 
    //the form we just don't add an annotation
    private int ID { get; private set; }
    
    [AutoGenerateProperty]
    [Display("First Name")]//These two anotations create one text box called "First Name"
    public string FirstName { get; set; }
    
    [AutoGenerateProperty]
    [Display("Last Name")]
    public string LastName{ get; set; }
    
    [AutoGenerateProperty]
    [Display("Birthday")]
    public DateTime Birthday{ get; set; }
    
    [AutoGenerateProperty]
    [Display("Semester")]
    public int Semester{ get; set; }
}
```

#### 3 - In the MainPage we can call your class like this:

```csharp
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        this.MyStudent = new Student()
        {
            FirstName = "Brus",
            LastName = "Medina",
            Birthday = new Datetime(),
            Semester = 6
        };
        //Version Stable 1.0.7
         autogenerator.DataContext = MyStudent;
        //Breaking change 1.0.8 -alpha
       // autogenerator.CurrentDataContext = MyStudent;
    }
    #region Property
    private Student myStudent;
    public Student MyStudent
    {
        get { return myStudent; }
        set { myStudent = value; }
    }
    #endregion
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
    }
}
```
#### 4 - Finally, we set up our XAML in the next manner:

```xaml
<Grid Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
    <autogenerator:AutoGenerator Margin="24" x:Name="autogenerator"   TitleForm="My Student"/>
</Grid>
```

#### 5 - Our view will look like this
![Alt text](http://s15.postimg.org/hr1s64a7f/capture.png "")


#### 6 - Master - Details & Validations using MVVM Validation Helpers(https://www.nuget.org/packages/MvvmValidation/2.0.2)
###### You can use your own validation logic. 

##### 7 - We need to implement IValidatable  and INotifyDataErrorInfo to our model.
```csharp
  public class Student : PropertyChangeBase, IValidatable, INotifyDataErrorInfo
    {

     
        

        private string name;

        [AutoGenerateProperty]
        [Display("Name")]
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

        private DateTime birthDay;

        [AutoGenerateProperty]
        [Display("Birthday")]
        public DateTime BirthDay
        {
            get { return birthDay; }
            set
            {
                birthDay = value;
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
```
##### 8 - Well here is nothing special just a class called Course to save use in our ObservableCollection<Course> to select the course for each Student

```csharp
 public  class Course:PropertyChangeBase
    {
        public string Name { get; set; }
    }
```        

##### 8 - The group that contains all the collection of our Students.
```csharp
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
```        
##### 9 - Here is our ViewModel as you can see in our Master-Detail scenario We need to decorate our property SelectedStudent to render the form.
```csharp
    public class MainPageViewModel: PropertyChangeBase
    {

       

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
                       BirthDay= new System.DateTime(1996,12,20),
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

     
    }
```        
##### 10 - Finally we created our View.
```xaml
 <Grid Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="277*" />
            <ColumnDefinition Width="652*" />
            <ColumnDefinition Width="351*" />
        </Grid.ColumnDefinitions>
        <Grid>
            <Grid.RowDefinitions>
                <RowDefinition Height="31*" />
                <RowDefinition Height="329*" />
            </Grid.RowDefinitions>
            <TextBlock FontSize="26">My Group</TextBlock>
            <ListView
                Grid.Row="1"
                ItemsSource="{Binding MyGroup.Students, Mode=TwoWay}"
                SelectedItem="{Binding SelectedStudent, Mode=TwoWay}">
                <ListView.ItemTemplate>
                    <DataTemplate>
                        <TextBlock Text="{Binding Name, Mode=TwoWay}" />
                    </DataTemplate>
                </ListView.ItemTemplate>
            </ListView>

        </Grid>
        <autogenerator:AutoGenerator
            Grid.Column="1"
            Margin="24"
            CurrentDataContext="{Binding SelectedStudent, Mode=TwoWay}"
            ValidationCollection="{Binding SelectedStudent.Validations, Mode=TwoWay}" />

        <ListView Grid.Column="2" Margin="12" ItemsSource="{Binding SelectedStudent.Validations,Mode=TwoWay,UpdateSourceTrigger=PropertyChanged}">
            <ListView.ItemTemplate>
                <DataTemplate>
                    <StackPanel>
                        <TextBlock Text="{Binding Label, Mode=TwoWay}" Foreground="Red" />
                        <TextBlock Text="{Binding ErrorMessage, Mode=TwoWay}" Margin="0,8,0,0"/>
                    </StackPanel>
                </DataTemplate>
            </ListView.ItemTemplate>
        </ListView>
    </Grid>
```

#### 11 - Our view will look like this
![Alt text](http://s17.postimg.org/q9e5ym1xb/validations.png "")
