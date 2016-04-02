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
        autogenerator.DataContext = MyStudent;
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

![]({{site.baseurl}}/http://i68.tinypic.com/wj97ya.png)
