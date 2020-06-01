using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;
using Repositories;
using Services.Contracts;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Logging;
using Models;
using Models.AdminModels;
using Newtonsoft.Json;

namespace Services
{
    public class UserServices: IUserServices
    {
        private readonly IMongoRepository _repository;
        private ILogger<UserServices> logger;
        
        public UserServices(IMongoRepository repository, ILogger<UserServices> logger)
        {
            _repository = repository;
        }
        public async Task<User> Authenticate(string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    return null;

                var user = await _repository.GetItemAsync<User>(u => u.Username == username);
                if (user == null)
                {
                    return null;
                }

                var passResult = new PasswordHasher().VerifyHashedPassword(user.Password, password);
                if (passResult.Equals(PasswordVerificationResult.Failed))
                    return null;

                // authentication successful so return user details without password
                user.Password = null;
                return user;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"UserService: Authenticate Failed {ex.Message}");
            }

            return null;
        }

        public async Task<User> RegisterUser(RegisterViewModel model)
        {
            try
            {
                var exist = await _repository.GetItemAsync<User>(e => e.Username == model.Username);
                if (exist != null)
                {
                    return null;
                }

                var id = Guid.NewGuid().ToString();
                var password = new PasswordHasher().HashPassword(model.Password);

                User user = new User
                {
                    Id = id,
                    Name = model.Name,
                    Username = model.Username,
                    Password = password,
                };
                await _repository.SaveAsync<User>(user);
                var response = await _repository.GetItemAsync<User>(e => e.Id == id);
                if (response != null)
                {
                    response.Password = null;
                    return response;
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Register Failed: {ex.Message}");
            }

            return null;

        }

        public ClaimsIdentity GetSecurityClaims(User userInfo, string authenticationType = null)
        {
            try
            {
                if (userInfo == null)
                {
                    return null;
                }

                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userInfo.Username),
                    new Claim(ClaimTypes.Name, $"{userInfo.Name}"),
                    new Claim("Name", $"{userInfo.Name}"),
                    new Claim("Username", $"{userInfo.Username}"),
                    new Claim("UserData", JsonConvert.SerializeObject(userInfo)),
                };

                if (!string.IsNullOrEmpty(authenticationType))
                {
                    return new ClaimsIdentity(claims, authenticationType);
                }

                return new ClaimsIdentity(claims);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"GetSecurityClaims Failed {ex.Message}");
            }

            return null;
        }
    }
}