using AutoMapper;
using TotechsIdentity.DataObjects;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TotechsIdentity.AppSettings;
using TotechsIdentity.Models;
using Microsoft.AspNetCore.Authorization;

namespace TotechsIdentity.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccessController : BaseController
    {
        private readonly UserManager _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccessController> _logger;
        private readonly IOptionsMonitor<JwtTokenConfig> _tokenConfigOptionsAccessor;
        private readonly IdentityContext _identityContext;
        private readonly IMapper _mapper;
        public AccessController(UserManager userManager, SignInManager<User> signInManager, 
                                ILogger<AccessController> logger, IOptionsMonitor<JwtTokenConfig> tokenConfigOptionsAccessor,
                                IdentityContext identityContext, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _tokenConfigOptionsAccessor = tokenConfigOptionsAccessor;
            _identityContext = identityContext;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] CreateUserDTO dto, CancellationToken cancellationToken = default)
        {
            using var transaction = await _identityContext.Database.BeginTransactionAsync(cancellationToken);

            var user = _mapper.Map<User>(dto);

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                _logger.LogError("Unable to create user {username}. Detail: {result}", dto.UserName, string.Join(Environment.NewLine, createResult.Errors.Select(e => e.Description)));
                await transaction.RollbackAsync(cancellationToken);
                return StatusCode(500);
            };

            var addToRoleResult = await _userManager.AddToRolesAsync(user, dto.Roles);
            if (!addToRoleResult.Succeeded)
                _logger.LogError("Unable to assign user {username} to roles {roles}. Result details: {result}", dto.UserName, string.Join(", ", dto.Roles), string.Join(Environment.NewLine, addToRoleResult.Errors.Select(e => e.Description)));

            //await _userManager.AddClaimAsync(user, new Claim("", ""));

            await transaction.CommitAsync(cancellationToken);
            return Ok(_mapper.Map<UserDTO>(user));
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user is null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!passwordCheck.Succeeded)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenConfig = _tokenConfigOptionsAccessor.CurrentValue;
            var token = await GenerateToken(user, tokenConfig);
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
            });
        }

        private async Task<string> GenerateToken(User user, JwtTokenConfig jwtTokenConfig /*DateTime expires*/)
        {
            var handler = new JwtSecurityTokenHandler();

            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            var identity = new ClaimsIdentity(
                new GenericIdentity(user.UserName, "TokenAuth"),
                new[] { new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
                        //new Claim("id", user.Id.ToString()), 
                        new Claim("permission", "true")}
                    .Union(roles.Select(role => new Claim(ClaimTypes.Role, role)))
                    .Union(claims)
                );

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenConfig.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = jwtTokenConfig.Issuer,
                Audience = "Intranet",
                SigningCredentials = creds,
                Subject = identity,
                Expires = DateTime.UtcNow.AddDays(1)
            });

            return handler.WriteToken(securityToken);
        }
    }
}
