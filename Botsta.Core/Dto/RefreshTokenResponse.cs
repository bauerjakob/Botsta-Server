using System;
namespace Botsta.Core.Dto
{
    public class RefreshTokenResponse : ErrorResponse
    {
        public string Token { get; set; }
    }
}
