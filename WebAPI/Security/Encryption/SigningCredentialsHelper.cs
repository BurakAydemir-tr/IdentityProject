using Microsoft.IdentityModel.Tokens;

namespace WebAPI.Security.Encryption
{
    public class SigningCredentialsHelper
    {
        public static SigningCredentials CreateSignginCredentials(SecurityKey securityKey)
        {
            return new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha512Signature);
        }
    }
}
