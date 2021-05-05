using System;
namespace Botsta.Server.Dto
{
    public class LoginResponse : ErrorResponse
    {
        public string RefreshToken { get; set; }
        public string Token { get; set; }
    }
}
