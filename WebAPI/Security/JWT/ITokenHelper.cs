using Entities.Concrete;

namespace WebAPI.Security.JWT
{
    public interface ITokenHelper
    {
        AccessToken CreateToken(AppUser appUser, List<AppRole> appRoles);
    }
}
