using static System.Formats.Asn1.AsnWriter;

namespace Collect_Go2._0.Models
{
    public abstract class Employee : User
    {
        public Store? Store { get; set; }

        protected Employee()
        {
        }

        protected Employee(
            int userId,
            string firstname,
            string lastname,
            string email,
            string password,
            Store store)
            : base(userId, firstname, lastname, email, password)
        {
            Store = store;
        }
    }
}