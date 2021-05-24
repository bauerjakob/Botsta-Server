using System;
namespace Botsta.Core.Dto
{
    public class ErrorResponse
    {
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
    }
}
