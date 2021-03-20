using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Botsta.DataStorage.Models;
using Botsta.Server.Extentions;
using Botsta.Server.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Botsta.Server.Middelware
{
    public class IdentityService : IIdentityService
    {
        private readonly ILogger<IdentityService> _logger;
        private readonly AppConfig _appConfig;
        private readonly IBotstaDbRepository _dbContext;


        private const int TOKEN_ITERATIONS = 10000;
        private static readonly TimeSpan TOKEN_LIFETIME = TimeSpan.FromMinutes(30);


        public IdentityService(ILogger<IdentityService> logger, AppConfig appConfig, IBotstaDbRepository dbContext)
        {
            _logger = logger;
            _appConfig = appConfig;
            _dbContext = dbContext;
        }

        public async Task<User> RegisterAsync(string username, string password)
        {
            (var hash, var salt) = HashPassword(password);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = username,
                Registerd = DateTimeOffset.Now,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            await _dbContext.AddUserToDb(user);

            return user;
        }

        public string Login(string username, string password)
        {
            User user = _dbContext.GetUserByUsername(username);

            if (user != null
                && username == user.Username
                && VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                return GenerateJwtToken(user);
            }

            return null;
        }

        string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.JwtSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("role", PoliciesExtentions.User),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _appConfig.JwtIssuer,
                audience: _appConfig.JwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TOKEN_LIFETIME),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            if (string.IsNullOrEmpty(enteredPassword))
            {
                throw new ArgumentException(nameof(enteredPassword));
            }
            else if (string.IsNullOrEmpty(storedHash))
            {
                throw new ArgumentException(nameof(storedHash));
            }
            else if (string.IsNullOrEmpty(storedSalt))
            {
                throw new ArgumentException(nameof(storedSalt));
            }

            var saltBytes = Convert.FromBase64String(storedSalt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(enteredPassword, saltBytes, TOKEN_ITERATIONS);
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256)) == storedHash;
        }


        private static byte[] GenerateSalt()
        {
            var bytes = new byte[128 / 8];
            var rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(bytes);
            return bytes;
        }

        public static (string hash, string salt) HashPassword(string password)
        {
            if (!CheckIfPasswordIsValid(password))
            {
                throw new ArgumentException(nameof(password));
            }

            var salt = GenerateSalt();

            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, TOKEN_ITERATIONS);
            var hashPassword = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));

            return (hashPassword, Convert.ToBase64String(salt));
        }

        private static bool CheckIfPasswordIsValid(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(password);
            }
            else if (password.Length < 8)
            {
                throw new ArgumentException(nameof(password));
            }
            else if (!Regex.IsMatch(password, "[A-Z]"))
            {
                throw new ArgumentException(nameof(password));
            }
            else if (!Regex.IsMatch(password, "[a-z]"))
            {
                throw new ArgumentException(nameof(password));
            }
            else if (!Regex.IsMatch(password, "\\d"))
            {
                throw new ArgumentException(nameof(password));
            }

            return true;
        }
    }
}

