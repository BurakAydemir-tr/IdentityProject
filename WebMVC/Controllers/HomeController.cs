using Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebMVC.Models;
using WebMVC.ViewModels;
using WebMVC.Extensions;

namespace WebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
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

            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl);
            }

            // Hesap kilitlenmişse ekrana döndürülecek uyarı burda işleniyor.
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() {"3 dakika boyunca giriş yapamazsınız." });
                return View();
            }

            ModelState.AddModelErrorList(new List<string>() { $"Email veya şifre yanlış",
                                            $"Başarısız giriş sayısı = {await _userManager.GetAccessFailedCountAsync(hasUser)}"});

            return View();
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


            if (identityResult.Succeeded)
            {
                TempData["SuccessMessage"] = "Üyelik işlemi başarıyla gerçekleşmiştir.";
                return RedirectToAction(nameof(HomeController.SignUp));
            }


            ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
            
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
