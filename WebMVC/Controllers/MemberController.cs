using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;

        public MemberController(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }




        /* Bu birinci yöntem çıkş için*/
        //public async Task<IActionResult> Logout()
        //{
        //    await _signInManager.SignOutAsync();

        //    return RedirectToAction("Index","Home");
        //}


        // Bu ikinci yöntem
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();

        }


        public IActionResult Index()
        {
            return View();
        }

    }
}
