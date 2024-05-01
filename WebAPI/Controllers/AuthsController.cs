using Entities.Concrete;
using Entities.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Security.JWT;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenHelper _tokenHelper;

        public AuthsController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenHelper tokenHelper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenHelper = tokenHelper;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(AppUserRegisterDto appUserRegisterDto)
        {
            var user = new AppUser 
            { 
                UserName = appUserRegisterDto.UserName,
                Email = appUserRegisterDto.Email,
                Gender=appUserRegisterDto.Gender,
                CreatedOn=DateTime.Now,
            };

            var result=await _userManager.CreateAsync(user,appUserRegisterDto.Password);
            var accessToken=CreateAccessToken(user);
            if (result.Succeeded)
            {
                return Ok(accessToken);
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(AppUserLoginDto appUserLoginDto)
        {
            var appUser=await _userManager.FindByEmailAsync(appUserLoginDto.Email);
            if (appUser != null)
            {
                //İlgili kullanıcıya dair önceden oluşturulmuş bir Cookie varsa siliyoruz.
                await _signInManager.SignOutAsync();

                /*Method overload’ları incelendiğinde isPersistent ve lockoutOnFailure parametrelerini alan bir method mevcut. isPersistent parametresine true argümanı verilirse oturum cookie’de belirtilen süre boyunca aksi durumda session kadar olacaktır. lockoutOnFailure parametresine true argümanı verilirse lockout yönetimi Identity API’a devrediliyor. Aksi durumdaysa kendi lockout akışımızı yukarıda belirtilen methodlar doğrultusunda yazabiliriz.*/
                var result = await _signInManager.PasswordSignInAsync(appUser,appUserLoginDto.Password,appUserLoginDto.Persistent,true);

                if (result.Succeeded)
                {
                    //Giriş işleminin başarılı olmasıyla, giriş işlemi başarılı olana değin yapılan hatalı girişleri sıfırlıyoruz.
                    await _userManager.ResetAccessFailedCountAsync(appUser);

                    //Hesap, önceki başarısız giriş denemeleriyle kilitlenmişse SetLockoutEndDateAsync methoduyla sıfırlanıyoruz.
                    await _userManager.SetLockoutEndDateAsync(appUser, null);

                    var accessToken=CreateAccessToken(appUser);

                    return Ok(accessToken.Result);
                }else if(result.IsLockedOut) //hesap kilitli mi değil mi kontrol ediyoruz.
                {
                    var lockoutEndUtc = await _userManager.GetLockoutEndDateAsync(appUser);
                    var timeleft=lockoutEndUtc.Value-DateTime.UtcNow;
                    return BadRequest($"Hesap kilitli {timeleft.Minutes} dakika sonra tekrar deneyiniz.");
                }
                else
                {
                    return BadRequest("Invalid e-mail or password.");
                }

            }
            return BadRequest("Kullanıcı bulunamadı.");
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [Authorize(AuthenticationSchemes ="Bearer" ,Roles ="Admin,Moderator")]
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(string id)
        {
            var claims=User.Claims;

            var result = await _userManager.FindByIdAsync(id);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest("Kullanıcı bulunamadı.");
        }

        private async Task<AccessToken> CreateAccessToken(AppUser appUser)
        {
            var roles = await _userManager.GetRolesAsync(appUser);
            List<AppRole> rolesList = new List<AppRole>();
            foreach (var role in roles)
            {
                rolesList.Add(new AppRole { Name = role });
            }
            var token= _tokenHelper.CreateToken(appUser, rolesList);
            return token;

        }
    }
}
