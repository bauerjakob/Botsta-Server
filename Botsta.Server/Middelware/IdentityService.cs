using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Botsta.DataStorage.Entities;
using Botsta.Server.Extentions;
using Botsta.Server.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Security.Principal;
using Botsta.DataStorage;

namespace Botsta.Server.Middelware
{
    public class IdentityService : IIdentityService
    {
        private static Random _random = new Random();

        private readonly ILogger<IdentityService> _logger;
        private readonly AppConfig _appConfig;
        private readonly IBotstaDbRepository _dbContext;
        private readonly TokenValidationParameters _tokenValidationParameters;


        private const int TOKEN_ITERATIONS = 10000;
        private static readonly TimeSpan TOKEN_LIFETIME = TimeSpan.FromMinutes(30);


        public IdentityService(ILogger<IdentityService> logger, AppConfig appConfig, IBotstaDbRepository dbContext, TokenValidationParameters tokenValidationParameters)
        {
            _logger = logger;
            _appConfig = appConfig;
            _dbContext = dbContext;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task<User> RegisterUserAsync(string username, string password)
        {
            (var hash, var salt) = HashPassword(password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Registerd = DateTimeOffset.Now,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            await _dbContext.AddUserToDb(user);

            return user;
        }

        public async Task<(string apiKey, Bot bot)> RegisterBotAsync(string botName, User owner, string webhookUrl = null)
        {
            var apiKey = GenerateApiKey();

            (var hash, var salt) = HashPassword(apiKey);

            var bot = new Bot
            {
                Id = Guid.NewGuid(),
                BotName = botName,
                Registerd = DateTimeOffset.Now,
                ApiKeyHash = hash,
                ApiKeySalt = salt,
                WebhookUrl = webhookUrl,
                Owner = owner
            };

            await _dbContext.AddBotToDbAsync(bot);

            return (apiKey, bot);
        }

        private string GenerateApiKey()
        {
            int length = _random.Next(31, 37);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-.,:+#*'?=)(/&%$§}[]}!´`@<>";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public string LoginUser(string username, string password)
        {
            var user = _dbContext.GetUserByUsername(username);

            if (user != null
                && VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                return GenerateJwtToken(user.Id.ToString(), PoliciesExtentions.User);
            }

            return null;
        }

        public string LoginBot(string botName, string apiKey)
        {
            var bot = _dbContext.GetBotByName(botName);

            if (bot != null
                && VerifyPassword(apiKey, bot.ApiKeyHash, bot.ApiKeySalt))
            {
                return GenerateJwtToken(bot.Id.ToString(), PoliciesExtentions.Bot);
            }

            return null;
        }

        string GenerateJwtToken(string subject, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.JwtSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, subject),
                new Claim("role", role),
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

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out validatedToken);
            return principal;
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

