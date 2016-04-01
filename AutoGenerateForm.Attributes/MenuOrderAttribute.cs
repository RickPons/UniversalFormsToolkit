
namespace AutoGenerateForm.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class |
                           System.AttributeTargets.Struct)]
    public class MenuOrderAttribute : System.Attribute
    {
        public int Order;
        public Enums.MenuOrderItem DefaultOrder;
        public MenuOrderAttribute(int _order)
        {
            Order = _order;
        }
        

        public MenuOrderAttribute(Enums.MenuOrderItem defaultOrder)
        {
            DefaultOrder = defaultOrder;
        }
    }
}