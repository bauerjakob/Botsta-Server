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
using Botsta.Server.Dto;

namespace Botsta.Server.Middelware
{
    public class IdentityService : IIdentityService
    {
        private static Random _random = new Random();

        private readonly ILogger<IdentityService> _logger;
        private readonly AppConfig _appConfig;
        private readonly IBotstaDbRepository _repository;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly TimeSpan _refreshTokenExpirationTimeSpan;
        private readonly TimeSpan _tokenExpirationTimeSpan;


        private const int TOKEN_ITERATIONS = 10000;

        public IdentityService(ILogger<IdentityService> logger, AppConfig appConfig, IBotstaDbRepository repository, TokenValidationParameters tokenValidationParameters)
        {
            _logger = logger;
            _appConfig = appConfig;
            _repository = repository;
            _tokenValidationParameters = tokenValidationParameters;
            _refreshTokenExpirationTimeSpan = TimeSpan.FromDays(_appConfig.RefreshTokenExpirationDays);
            _tokenExpirationTimeSpan = TimeSpan.FromMinutes(_appConfig.TokenExpirationMinutes);
        }

        public async Task<User> RegisterUserAsync(string username, string password)
        {
            (var hash, var salt) = HashPassword(password);

            var user = await _repository.AddUserToDbAsync(username, hash, salt);

            return user;
        }

        public async Task<(string apiKey, Bot bot)> RegisterBotAsync(string botName, User owner, string webhookUrl = null)
        {
            var apiKey = GenerateApiKey();

            (var hash, var salt) = HashPassword(apiKey);

            var bot = await _repository.AddBotToDbAsync(owner, botName, hash, salt);

            return (apiKey, bot);
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(string refreshToken)
        {

            var claims = ValidateToken(refreshToken);
            return await RefreshTokenAsync(claims);
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(ClaimsPrincipal claims)
        {
            var practicantId = claims.Claims.GetSubject();
            var chatPracticant = await _repository.GetChatPracticantAsync(Guid.Parse(practicantId));

            var generatedToken = GenerateJwtToken(practicantId,
                chatPracticant.Type == PracticantType.User ? PoliciesExtentions.User : PoliciesExtentions.Bot, DateTime.UtcNow.Add(_tokenExpirationTimeSpan));

            return new RefreshTokenResponse
            {
                Token = generatedToken.token
            };
        }


        private string GenerateApiKey()
        {
            int length = _random.Next(31, 37);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-.,:+#*'?=)(/&%$§}[]}!´`@<>";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public async Task<LoginResponse> LoginAsync(string name, string secret)
        {
            var practicant = await _repository.GetChatPracticantAsync(name);

            if (practicant != null
                && VerifyPassword(secret, practicant.SecretHash, practicant.SecretSalt))
            {

                var refreshToken = GenerateJwtToken(
                    practicant.Id.ToString(),
                    PoliciesExtentions.RefreshToken,
                    DateTime.UtcNow.Add(_refreshTokenExpirationTimeSpan));

                var token = GenerateJwtToken(
                    practicant.Id.ToString(),
                    practicant.Type == PracticantType.User ?
                        PoliciesExtentions.User :
                        PoliciesExtentions.Bot,
                    DateTime.UtcNow.Add(_tokenExpirationTimeSpan));

                return new LoginResponse
                {
                    Token = token.token,
                    RefreshToken = refreshToken.token,
                };
            }

            return new LoginResponse
            {
                HasError = true,
                ErrorCode = "login.failed",
                ErrorMessage = "Login failed"
            };
        }

        (string token, Guid tokenId) GenerateJwtToken(string subject, string role, DateTime? expireDate)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.JwtSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenId = Guid.NewGuid();
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, subject),
                new Claim("role", role),
                new Claim(JwtRegisteredClaimNames.Jti, tokenId.ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _appConfig.JwtIssuer,
                audience: _appConfig.JwtAudience,
                claims: claims,
                expires: expireDate,
                signingCredentials: credentials
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), tokenId);
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

