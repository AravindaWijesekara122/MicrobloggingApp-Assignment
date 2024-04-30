using BusinessLogicLayer.IService;
using DataAccessLayer.Entities;
using DataAccessLayer.IRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<bool> RegisterUserAsync(User user)
        {
            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(user.Email))
            {
                return false;
            }

            // Add user to the database
            return await _userRepository.AddUserAsync(user);
        }

        public async Task<string> AuthenticateUserAsync(string email, string password)
        {
            // Find user by email
            var user = await _userRepository.GetUserByEmailAsync(email);

            // Verify password
            if (user == null || !VerifyPasswordHash(password, user.Password))
            {
                return null;
            }

            // Generate JWT token
            return GenerateJwtToken(user);
        }

        private bool VerifyPasswordHash(string password, string passwordHash)
        {
    
            return password == passwordHash;
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
