using MongoDB.Driver;
using RecipeApi.Models;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace RecipeApi.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _userCollection;
        private readonly IConfiguration _configuration;

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
            var client = new MongoClient(configuration["MongoDbSettings:ConnectionString"]);
            var database = client.GetDatabase(configuration["MongoDbSettings:DatabaseName"]);
            _userCollection = database.GetCollection<User>("Users");
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _userCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User> RegisterUser(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _userCollection.InsertOneAsync(user);
            return user;
        }

        public async Task<string?> AuthenticateUser(UserLoginDto userData)
        {
            var user = await GetUserByEmail(userData.Email);
            if (user==null || !BCrypt.Net.BCrypt.Verify(userData.Password, user.Password))
            {
                return null;
            }

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]!);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id!),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpirationInMinutes"]!)),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

         public async Task<bool> UpdateUser(string id, UserUpdateDto userUpdateDto)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var update = Builders<User>.Update
                .Set(u => u.Username, userUpdateDto.Username)
                .Set(u => u.Email, userUpdateDto.Email);

            if (!string.IsNullOrEmpty(userUpdateDto.Password))
            {
                update = update.Set(u => u.Password, BCrypt.Net.BCrypt.HashPassword(userUpdateDto.Password));
            }

            var result = await _userCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

            public async Task<bool> DeleteUser(string id)
        {
            var result = await _userCollection.DeleteOneAsync(u => u.Id == id);
            return result.DeletedCount > 0;
        }

        
    }
}
