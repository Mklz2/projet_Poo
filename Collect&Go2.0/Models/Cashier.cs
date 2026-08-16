namespace Collect_Go2._0.Models
{
    public class Cashier : Employee
    {
        public override string UserType => "cashier";

        public Cashier(
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
        }
    }
}
