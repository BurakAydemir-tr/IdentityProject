using Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebMVC.Models;
using WebMVC.ViewModels;
using WebMVC.Extensions;
using WebMVC.Services;
using System.Security.Claims;

namespace WebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult SignUp()
        {
            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel request, string? returnUrl=null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            /* returnUrl Kişi kullanıcılara ait bir yere girmeye çalıştığında returnUrl ile girmeye çalıştıtğı yeri tutup login oldujktan sonra oraya yönlendiriyoruz.*/
            returnUrl = returnUrl ?? Url.Action("Index", "Home");

            var hasUser = await _userManager.FindByEmailAsync(request.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email veya şifre yanlış");
                return View();
            }

            /* - isPersistent kullanıcı bilgilerinin cookie de tutulup tutulmayacağı anlamına geliyor.
             burada request.RemmeberMe ile true geçerek tutulmasını istiyoruz.
            
                - lockoutOnFailure kullanıcı 3 kere yanlış giriş yapıldığında hesabı kitleyip kitlemeyeceğimizi soruyor. True dersek devreye giriyor. False deyince girmiyor.
             */
            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, request.Password, request.RememberMe, true);

            // Hesap kilitlenmişse ekrana döndürülecek uyarı burda işleniyor.
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "3 dakika boyunca giriş yapamazsınız." });
                return View();
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelErrorList(new List<string>() { $"Email veya şifre yanlış",
                                            $"Başarısız giriş sayısı = {await _userManager.GetAccessFailedCountAsync(hasUser)}"});

                return View();   
            }

            //Doğum tarihini olup olmadığını kontrol ediyoruz.
            if (hasUser.BirthDate.HasValue)
            {
                /*Doğum tarihi varsa cookie ye birthdate claimi ve değerini ekliyoruz.
                 Bunu claim i yaşa bağlı olarak bazı sayfalara erişim izni vereceğiz yada vermeyeceğiz.*/
                await _signInManager.SignInWithClaimsAsync(hasUser, request.RememberMe,
                    new[] { new Claim("birthdate", hasUser.BirthDate.Value.ToString())});
            }

            return Redirect(returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var identityResult = await _userManager.CreateAsync(new AppUser()
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber=request.Phone,
                Gender=Gender.Male,
                CreatedOn=DateTime.Now
            }, request.Password);


            if (!identityResult.Succeeded)
            {
                ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
                return View();
            }

            /*Bir sayfaya belirli bir süre erişmek için User Claim ekleme*/
            //Borsa sayfası için 10 günlüğüne erişebilecek bir claim oluşturuyoruz.
            var exchangeExpireClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString());
            var user = await _userManager.FindByNameAsync(request.UserName);
            //Oluşturulan claim i UserClaims tablosuna ekliyoruz.
            var claimResult = await _userManager.AddClaimAsync(user, exchangeExpireClaim);

            if (!claimResult.Succeeded)
            {
                ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
                return View();
            }


            TempData["SuccessMessage"] = "Üyelik işlemi başarıyla gerçekleşmiştir.";
            return RedirectToAction(nameof(HomeController.SignUp));

        }


        public IActionResult ForgetPassword()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel request)
        {
            var hasUser = await _userManager.FindByEmailAsync(request.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(String.Empty, "Bu email adresine sahip kullanıcı bulunamamıştır");
                return View();
            }

            // Password resetlemek için bir token üretiyoruz.
            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);

            var passwordResetLink = Url.Action("ResetPassword", "Home", new {userId = hasUser.Id, Token = passwordResetToken}, HttpContext.Request.Scheme);

            //Örnek Link https://localhost:7104?userId=1234&token=fgdtrkbogtkdlfjeo

            //Email Service
            await _emailService.SendResetPasswordEmail(passwordResetLink, hasUser.Email);

            TempData["SuccessMessage"] = "Şifre yenileme linki eposta adresinize gönderilmiştir.";

            return RedirectToAction(nameof(ForgetPassword));
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request)
        {
            var userId = TempData["userId"];
            var token = TempData["token"];

            if (userId == null || token == null)
            {
                throw new Exception("Bir hata meydana geldi.");
            }

            var hasUser = await _userManager.FindByIdAsync(userId.ToString());

            if (hasUser == null)
            {
                ModelState.AddModelError(String.Empty, "Kullanıcı bulunamamıştır");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(hasUser, token.ToString(), request.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Şifreniz başarıyla yenilenmiştir.";
            }
            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(error => error.Description).ToList());
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
