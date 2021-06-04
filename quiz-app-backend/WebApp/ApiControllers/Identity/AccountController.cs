using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PublicApi.DTO.v1.Account;
using AppRole = Domain.App.Identity.AppRole;
using AppUser = Domain.App.Identity.AppUser;

namespace WebApp.ApiControllers.Identity
{
    /// <summary>
    /// API controller for registering and logging in.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for controller.
        /// </summary>
        /// <param name="signInManager">Sign in manager</param>
        /// <param name="userManager">User manager</param>
        /// <param name="logger">Logger</param>
        /// <param name="configuration">Configuration</param>
        /// <param name="roleManager">Role manager</param>
        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager,
            ILogger<AccountController> logger, IConfiguration configuration,
            RoleManager<AppRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Try to log in user
        /// </summary>
        /// <param name="dto">Login dto</param>
        /// <returns>JWT response with token</returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PublicApi.DTO.v1.Account.JwtResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] Login dto)
        {
            var appUser = await _userManager.FindByEmailAsync(dto.Email);
            if (appUser == null)
            {
                _logger.LogWarning("WebApi login failed. User {User} not found", dto.Email);
                return NotFound(new Message("User/Password problem!"));
            }

            if (appUser.LockoutEnd != null &&
                appUser.LockoutEnd > DateTime.Now)
                return NotFound(
                    new Message($"You are locked out of you account until \n {appUser.LockoutEnd.Value.Date}"));

            var result = await _signInManager.CheckPasswordSignInAsync(appUser, dto.Password, false);
            if (result.Succeeded)
            {
                var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(appUser);
                var jwt = Extensions.Base.IdentityExtensions.GenerateJwt(
                    claimsPrincipal.Claims,
                    _configuration["JWT:Key"],
                    _configuration["JWT:Issuer"],
                    _configuration["JWT:Issuer"],
                    DateTime.Now.AddDays(_configuration.GetValue<int>("JWT:ExpireDays"))
                );
                _logger.LogInformation("WebApi login. User {User}", dto.Email);
                return Ok(new JwtResponse
                {
                    Token = jwt,
                    Firstname = appUser.Firstname,
                    Lastname = appUser.Lastname,
                });
            }

            _logger.LogWarning("WebApi login failed. User {User} - bad password", dto.Email);
            return NotFound(new Message("User/Password problem!"));
        }


        /// <summary>
        /// Try to register new user.
        /// </summary>
        /// <param name="dto">Register dto</param>
        /// <returns>JWT response with token for new user</returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PublicApi.DTO.v1.Account.JwtResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] Register dto)
        {
            var appUser = await _userManager.FindByEmailAsync(dto.Email);
            if (appUser != null)
            {
                _logger.LogWarning(" User {User} already registered", dto.Email);
                return BadRequest(new Message("User already registered"));
            }

            appUser = new Domain.App.Identity.AppUser()
            {
                Email = dto.Email,
                Firstname = dto.Firstname,
                Lastname = dto.Lastname,
                UserName = dto.Email
            };
            var result = await _userManager.CreateAsync(appUser, dto.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} created a new account with password", appUser.Email);

                var user = await _userManager.FindByEmailAsync(appUser.Email);
                if (user != null)
                {
                    var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
                    var jwt = Extensions.Base.IdentityExtensions.GenerateJwt(
                        claimsPrincipal.Claims,
                        _configuration["JWT:Key"],
                        _configuration["JWT:Issuer"],
                        _configuration["JWT:Issuer"],
                        DateTime.Now.AddDays(_configuration.GetValue<int>("JWT:ExpireDays"))
                    );
                    _logger.LogInformation("WebApi login. User {User}", dto.Email);
                    return Ok(new JwtResponse()
                    {
                        Token = jwt,
                        Firstname = appUser.Firstname,
                        Lastname = appUser.Lastname,
                    });
                }

                _logger.LogInformation("User {Email} not found after creation", appUser.Email);
                return BadRequest(new Message("User not found after creation!"));
            }

            var errors = result.Errors.Select(error => error.Description).ToList();
            return BadRequest(new Message() {Messages = errors});
        }
    }
}