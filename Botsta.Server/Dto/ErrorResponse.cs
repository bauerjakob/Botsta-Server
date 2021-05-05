using System;
namespace Botsta.Server.Dto
{
    public class ErrorResponse
    {
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
    }
}
