using System;
namespace Botsta.Server.Dto
{
    public class RefreshTokenResponse : ErrorResponse
    {
        public string Token { get; set; }
    }
}
