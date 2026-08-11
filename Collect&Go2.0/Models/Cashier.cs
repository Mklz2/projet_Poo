using static System.Formats.Asn1.AsnWriter;

namespace Collect_Go2._0.Models
{
    public class Cashier : Employee
    {
        public Cashier()
        {
            UserType = "cashier";
        }

        public Cashier(
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
            UserType = "cashier";
        }
    }
}