using System;

namespace Botsta.Server.Configuration
{
    public class AppConfig
    {
        public string JwtIssuer { get; set; }
        public string JwtAudience { get; set; }
        public string JwtSecret { get; set; }
    }
}
