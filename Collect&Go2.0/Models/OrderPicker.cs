namespace Collect_Go2._0.Models
{
    public class OrderPicker : Employee
    {
        public OrderPicker(
            int userId,
            string firstname,
            string lastname,
            string email,
            string password,
            Store store,
            DateTime hiringDate)
            : base(
                userId,
                firstname,
                lastname,
                email,
                password,
                store,
                hiringDate)
        {
            UserType = "orderpicker";
        }
    }
}
