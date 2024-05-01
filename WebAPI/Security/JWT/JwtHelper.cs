using Entities.Concrete;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Security.JWT
{
    public class JwtHelper : ITokenHelper
    {
        readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AccessToken CreateToken(AppUser appUser, List<AppRole> appRoles)
        {
            AccessToken token = new AccessToken();

            //Security key in simetriğini alıyoruz.
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["TokenOptions:SecurityKey"]));

            //Şifrelenemiş kimliği oluşturuyoruz.
            SigningCredentials signingCredentials=new(securityKey, SecurityAlgorithms.HmacSha256);

            token.Expiration = DateTime.UtcNow.AddMinutes(10);

            //Oluşturulacak token ayarlarını veriyoruz.
            JwtSecurityToken jwtSecurityToken = new(
                    audience: _configuration["TokenOptions:Audience"],
                    issuer: _configuration["TokenOptions:Issuer"],
                    signingCredentials: signingCredentials,
                    expires: token.Expiration,
                    notBefore: DateTime.UtcNow,
                    claims: SetClaims(appUser,appRoles)
                );

            JwtSecurityTokenHandler tokenHandler = new();
            token.Token=tokenHandler.WriteToken(jwtSecurityToken);

            return token;
        }

        private IEnumerable<Claim> SetClaims(AppUser appUser, List<AppRole> appRoles)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, appUser.UserName));
            claims.Add(new Claim(ClaimTypes.Email, appUser.Email));
            foreach (var role in appRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }
            return claims;
        }
    }
}
