using DataAccessLayer.Entities;
using DataAccessLayer.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbcontext;

        public UserRepository(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbcontext.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> AddUserAsync(User user)
        {
            try
            {
                await _dbcontext.Users.AddAsync(user);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                // Handle exceptions (e.g., database errors)
                return false;
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _dbcontext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
