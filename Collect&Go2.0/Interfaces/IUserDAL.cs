using Collect_Go2._0.Models;

namespace Collect_Go2._0.Interfaces
{
    public interface IUserDAL
    {
        Task<User?> GetByEmailAndPasswordAsync(
            string email,
            string password);

        Task<bool> EmailExistsAsync(string email);

        Task CreateAsync(Client client);
    }
}


//incomplet 