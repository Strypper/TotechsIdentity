using AutoMapper;
using TotechsIdentity.DataObjects;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TotechsIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using TotechsIdentity.Services.IService;
using TotechsIdentity.Constants;

namespace TotechsIdentity.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccessController : BaseController
    {
        private readonly IMapper                         _mapper;
        private readonly UserManager                     _userManager;
        private readonly IEmailService                   _emailService;
        private readonly ITokenService                   _tokenService;
        private readonly IdentityContext                 _identityContext;
        private readonly RoleManager<Role>               _roleManager;
        private readonly SignInManager<User>             _signInManager;
        private readonly ILogger<AccessController>       _logger;
        public AccessController(IMapper mapper, 
                                UserManager userManager,
                                IEmailService emailService,
                                ITokenService tokenService,
                                RoleManager<Role> roleManager,
                                ILogger<AccessController> logger,
                                SignInManager<User> signInManager, 
                                IdentityContext identityContext)
        {
            _logger          = logger;
            _mapper          = mapper;
            _userManager     = userManager;
            _roleManager     = roleManager;
            _tokenService    = tokenService;
            _emailService    = emailService;
            _signInManager   = signInManager;
            _identityContext = identityContext;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] CreateUserDTO dto, CancellationToken cancellationToken = default)
        {
            using var transaction = await _identityContext.Database.BeginTransactionAsync(cancellationToken);

            var user = _mapper.Map<User>(dto);
            user.DateJoin = DateTime.UtcNow;

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                _logger.LogError("Unable to create user {username}. Detail: {result}", dto.UserName, string.Join(Environment.NewLine, createResult.Errors.Select(e => e.Description)));
                await transaction.RollbackAsync(cancellationToken);
                return StatusCode(500);
            };

            foreach (var roleId in dto.Roles)
            {
                var role = _roleManager.FindByIdAsync(roleId); 
                if(role.Result != null)
                {
                    var addToRoleResult = await _userManager.AddToRoleAsync(user, role.Result.NormalizedName);
                    if (!addToRoleResult.Succeeded)
                        _logger.LogError("Unable to assign user {username} to roles {roles}. Result details: {result}", dto.UserName, string.Join(", ", dto.Roles), string.Join(Environment.NewLine, addToRoleResult.Errors.Select(e => e.Description)));
                }
                else
                {
                    _logger.LogError($"This role id: {roleId} does not exist");
                    return BadRequest($"This role id: {roleId} does not exist");
                }
            }

            //await _userManager.AddClaimAsync(user, new Claim("", ""));

            await transaction.CommitAsync(cancellationToken);

            await SendEmailConfirmation(user);

            return Ok(_mapper.Map<UserDTO>(user));
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user is null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var userRoles = await _userManager.GetRolesAsync(user);
            var userDTO = _mapper.Map<UserDTO>(user);
            userDTO.Roles = userRoles.ToArray();

            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!passwordCheck.Succeeded)
                return BadRequest(new { message = "Username or password is incorrect" });

            var token         = await _tokenService.GenerateToken(user);
            var refresh_token = Guid.NewGuid().ToString().Replace("-", "");

            var requestAt = DateTime.UtcNow;
            var expiresIn = Math.Floor((requestAt.AddDays(1) - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
            //var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                user.Id,
                requestAt,
                expiresIn,
                accessToken = token,
                refresh_token,
                userInfo = userDTO
            });
        }


        [AllowAnonymous]
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string guid, string token)
        {
            if (string.IsNullOrWhiteSpace(guid) || string.IsNullOrWhiteSpace(token))
                return NotFound();


            var user = await _userManager.FindByGuidAsync(guid);
            if (user is null)
                return NotFound();


            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);


            var result = await _userManager.ConfirmEmailAsync(user, normalToken);
            if (!result.Succeeded)
                return BadRequest(result);

            return Content(EmailConstants.SuccessHtmlTemplate, 
                           EmailConstants.ContentType);
        }

        private async Task SendEmailConfirmation(User user)
        {
            // Encode confirmation token
            var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var validEmailToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmEmailToken));

            // Generate URL
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";
            string confirmUrl = $"{baseUrl}/api/access/confirmEmail/confirm-email?guid={user.Guid}&token={validEmailToken}";

            await _emailService.SendEmailConfirmation(confirmUrl, user.UserName, user.Email);
        }
    }
}
