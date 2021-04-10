using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Botsta.Server.Configuration;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Botsta.DataStorage.Entities;
using System.Security.Cryptography;
using Botsta.Server.Middelware;
using Microsoft.AspNetCore.Authorization;

namespace Botsta.Server.Controllers
{
    [ApiController]
    [Route("/api")]
    public class IdentityController : ControllerBase
    {
        private readonly ILogger<IdentityController> _logger;
        private readonly IIdentityService _identityService;


        public IdentityController(ILogger<IdentityController> logger, IIdentityService identityService
            )
        {
            _logger = logger;
            _identityService = identityService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register()
        {
            var username = "Lul";
            var password = "Ab1asiuhasiudsgk";

            try
            {
                await _identityService.RegisterUserAsync(username, password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return BadRequest();
            }

            return Ok();

        }


        [HttpPost("login")]
        public ActionResult Login()
        {
            var authorizationHeader = "Bearer THVsOkFiMWFzaXVoYXNpdWRzZ2s="; // Request.Headers["Authorization"].First();
            var key = authorizationHeader.Split(' ')[1];
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(key)).Split(':');
            var username = credentials[0];
            var password = credentials[1];
            var token = _identityService.LoginUser(username, password);

            return Ok(new { token });
        }
    }
}
