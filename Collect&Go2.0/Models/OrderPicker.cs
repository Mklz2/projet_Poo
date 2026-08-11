namespace Collect_Go2._0.Models
{
    public class OrderPicker : Employee
    {
        public OrderPicker()
        {
            UserType = "orderpicker";
        }

        public OrderPicker(
            int userId,
            string firstname,
            string lastname,
            string email,
            string password,
            Store store)
            : base(
                userId,
                firstname,
                lastname,
                email,
                password,
                store)
        {
            UserType = "orderpicker";
        }
    }
}