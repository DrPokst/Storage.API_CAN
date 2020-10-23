using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Storage.API.Models;
using Microsoft.EntityFrameworkCore;
using Storage.API_CAN.Models;

namespace Storage.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null)
                return null;

           //if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            //    return null; 

            return user; 
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
              using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                    var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                    for (int i = 0; i < computedHash.Length; i++)
                    {
                        if (computedHash[i] != passwordHash[i]) 
                            return false;
                    }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;

        }
        public async Task<UserPhoto> RegisterPhoto(UserPhoto userPhoto)
        {
            await _context.UserPhoto.AddAsync(userPhoto);
            await _context.SaveChangesAsync();

            return userPhoto;
        }
        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.UserPhoto)
                                                          .FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                    passwordSalt = hmac.Key;  
                    passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        public async Task<UserPhoto> GetPhoto(int id)
        {
            var photo = await _context.UserPhoto.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        private object GetBytes(string password)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == username))
                return true;
            
            return false;

        }
    }
}
