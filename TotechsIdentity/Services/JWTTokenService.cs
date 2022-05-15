using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using Repositories.Extensions;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TotechsIdentity.AppSettings;
using TotechsIdentity.Constants;
using TotechsIdentity.Services.IService;

namespace TotechsIdentity.Services
{
    public class JWTTokenService : ITokenService
    {
        private readonly UserManager     _userManager;
        private readonly JwtTokenConfig  _tokenConfig;
        private readonly IdentityContext _identityContext;
        public JWTTokenService(UserManager                     userManager
                              ,IOptionsMonitor<JwtTokenConfig> tokenConfigOptionsAccessor
                              ,IdentityContext                 identityContext)
        {
            _userManager     = userManager;
            _tokenConfig     = tokenConfigOptionsAccessor.CurrentValue;
            _identityContext = identityContext;
        }
        public async Task<string> GenerateToken(User user)
        {
            var handler = new JwtSecurityTokenHandler();

            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            var projectPermissions = await _identityContext.ProjectPermissions.Where(projectPermission => projectPermission.RequestUser.Id == user.Id).ToArrayAsync();

            var identity = new ClaimsIdentity(
                new GenericIdentity(user.UserName, JwtTokenConstants.GenericIdentityType),
                new[] { new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())}
                    .Union(roles.Select(role => new Claim(ClaimTypes.Role, role)))
                    .Union(claims)
                    .Union(projectPermissions.ClaimsExtensions())
                );

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfig.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer             = _tokenConfig.Issuer,
                Audience           = TotechsConstants.ServiceName,
                SigningCredentials = creds,
                Subject            = identity,
                Expires            = DateTime.UtcNow.AddDays(1)
            });

            return handler.WriteToken(securityToken);
        }
    }
}
