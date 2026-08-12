namespace Collect_Go2._0.Models
{
    public abstract class Employee : User
    {
        public Store? Store { get; set; }

        public DateTime HiringDate { get; set; }

        protected Employee(
            int userId,
            string firstname,
            string lastname,
            string email,
            string password,
            Store store,
            DateTime hiringDate)
            : base(userId, firstname, lastname, email, password)
        {
            Store = store;
            HiringDate = hiringDate;
        }
    }
}
