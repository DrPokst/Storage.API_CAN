using System.Threading.Tasks;
using Storage.API.Models;
using Storage.API_CAN.Models;

namespace Storage.API.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User user, string password);
         Task<User> Login(string username, string password);   
         Task<UserPhoto> GetPhoto(int id);
         Task<User> GetUser(int id);
         Task<bool> UserExists(string username);
    }
}