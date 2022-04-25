using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Entities;
using OBPostupyApi.Enums;
using OBPostupyApi.Models;
using OBPostupyApi.Services;
using OBPostupyApi.Settings;
using System;
using System.Text;
using System.Threading.Tasks;

namespace OBPostupyApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly FrontEndSettings _frontEndSettings;
        private readonly ILogger<AuthenticateController> _logger;

        public AuthenticateController(
            UserManager<User> userManager, 
            ITokenService tokenService, 
            IEmailService emailService, 
            IOptions<FrontEndSettings> options,
            ILogger<AuthenticateController> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _frontEndSettings = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.UserName);
            if (user == null) return BadRequest(model.UserName);

            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var expiration = model.IsPermanent ? DateTime.UtcNow.AddDays(14) : DateTime.UtcNow.AddHours(12);
                return Ok(await _tokenService.GenerateTokenAsync(user, expiration));
            }

            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("{@model}", model);

            var user = new User { 
                UserName = model.Email,
                Email = model.Email, 
                FirstName = model.FirstName, 
                LastName = model.LastName 
            };

            var registerResult = await _userManager.CreateAsync(user, model.Password);
            var addRoleResult = await _userManager.AddToRoleAsync(user, Role.User.ToString());

            if (registerResult.Succeeded && addRoleResult.Succeeded)
                return Ok();

            return BadRequest(new { register = registerResult.Errors });
        }

        [HttpPost("password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return BadRequest();
            }

            var changePassword = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (changePassword.Succeeded)
            {
                return Ok();
            }

            return BadRequest(changePassword.Errors);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            User user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest();
            }

            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(await _userManager.GeneratePasswordResetTokenAsync(user)));

            var uri = _frontEndSettings.Uri;
            await _emailService.SendEmailAsync(
                model.Email, 
                "Reset hesla na OB Postupech",
                $"Pro reset hesla přejděte na odkaz: <a href=\"{uri}/reset-hesla?email={model.Email}&token={token}\">link</a>");

            return Ok();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return BadRequest();
            }

            var token = Encoding.UTF8.GetString(Convert.FromBase64String(model.Token));

            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
            if (resetPasswordResult.Succeeded)
            {
                return Ok();
            }

            return BadRequest(resetPasswordResult.Errors);
        }
    }
}
