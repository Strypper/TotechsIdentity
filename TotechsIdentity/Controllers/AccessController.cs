using AutoMapper;
using DataObjects;
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

namespace TotechsIdentity.Controllers
{
    [Route("api/authentication")]
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
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserDTO userDTO, CancellationToken cancellationToken = default)
        {
            using var transaction = await _identityContext.Database.BeginTransactionAsync(cancellationToken);

            var user = _mapper.Map<User>(userDTO);

            var createResult = await _userManager.CreateAsync(user, "Welkom01");
            if (!createResult.Succeeded)
            {
                _logger.LogError("Unable to create user {username}. Detail: {result}", userDTO.UserName, string.Join(Environment.NewLine, createResult.Errors.Select(e => e.Description)));
                await transaction.RollbackAsync(cancellationToken);
                return StatusCode(500);
            };

            var addToRoleResult = await _userManager.AddToRolesAsync(user, userDTO.Roles);
            if (!addToRoleResult.Succeeded)
                _logger.LogError("Unable to assign user {username} to roles {roles}. Result details: {result}", userDTO.UserName, string.Join(", ", userDTO.Roles), string.Join(Environment.NewLine, addToRoleResult.Errors.Select(e => e.Description)));

            await _userManager.AddClaimAsync(user, new Claim("", ""));
            await _userManager.AddClaimAsync(user, new Claim("", ""));

            await transaction.CommitAsync(cancellationToken);
            return Ok(_mapper.Map<UserDTO>(user));
        }

        private async Task<string> GenerateToken(User user, JwtTokenConfig jwtTokenConfig, DateTime expires)
        {
            var handler = new JwtSecurityTokenHandler();

            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            var identity = new ClaimsIdentity(
                new GenericIdentity(user.UserName, "TokenAuth"),
                new[] { new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), new Claim("id", user.Id.ToString()) }
                    .Union(roles.Select(role => new Claim(ClaimTypes.Role, role)))
                    .Union(claims)
                );

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenConfig.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var securityToken = handler.CreateToken(new SecurityTokenDescriptor 
            {
                Issuer = jwtTokenConfig.Issuer,
                Audience = jwtTokenConfig.Issuer,
                SigningCredentials = creds,
                Subject = identity,
                Expires = expires
            });

            return handler.WriteToken(securityToken);
        }
    }
}
