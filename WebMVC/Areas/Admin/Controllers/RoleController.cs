using Azure.Core;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMVC.Areas.Admin.Models;
using WebMVC.Extensions;

namespace WebMVC.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        public RoleController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Select(x => new RoleViewModel()
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();

            return View(roles);
        }

        [Authorize(Roles = "role-action")]
        public IActionResult RoleCreate()
        {
            return View();
        }

        [Authorize(Roles = "role-action")]
        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleCreateViewModel request)
        {
            var result = await _roleManager.CreateAsync(new AppRole() { Name = request.Name });

            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }

            TempData["SuccessMessage"] = "Rol oluşturulmuştur.";
            return RedirectToAction("Index","Role");
        }

        [Authorize(Roles = "role-action")]
        public async Task<IActionResult> RoleUpdate(int id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id.ToString());

            if (roleToUpdate == null)
            {
                throw new Exception("Güncellenecek rol bulunamamıştır.");
            }

            return View(new RoleUpdateViewModel()
            {
                Id = roleToUpdate.Id,
                Name = roleToUpdate!.Name!
            });
        }

        [Authorize(Roles = "role-action")]
        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel request)
        {
            var roleToUpdate =await _roleManager.FindByIdAsync(request.Id.ToString());

            if (roleToUpdate == null)
            {
                throw new Exception("Güncellenecek rol bulunamamıştır.");
            }

            roleToUpdate.Name = request.Name;

            var updateResult = await _roleManager.UpdateAsync(roleToUpdate);

            if (!updateResult.Succeeded)
            {
                ModelState.AddModelErrorList(updateResult.Errors);
                return View();
            }

            ViewData["SuccessMessage"] = "Rol bilgisi güncellenmiştir.";

            return View();
        }

        [Authorize(Roles = "role-action")]
        public async Task<IActionResult> RoleDelete(int id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(id.ToString());

            if (roleToDelete == null)
            {
                throw new Exception("Güncellenecek rol bulunamamıştır.");
            }


            var deleteResult = await _roleManager.DeleteAsync(roleToDelete);

            if (!deleteResult.Succeeded)
            {
                ModelState.AddModelErrorList(deleteResult.Errors);
                return View();
            }

            TempData["SuccessMessage"] = "Rol silinmiştir.";
            return RedirectToAction("Index","Role");
        }

        [Authorize(Roles = "role-action")]
        public async Task<IActionResult> AssignRoleToUser(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id);
            ViewBag.userId = id;
            var roles = await _roleManager.Roles.ToListAsync();

            var userRoles = await _userManager.GetRolesAsync(currentUser);

            var roleViewModelList = new List<AssignRoleToUserViewModel>();


            foreach (var role in roles)
            {
                var assignRoleToUserViewModel = new AssignRoleToUserViewModel()
                {
                    Id = role.Id,
                    Name = role.Name
                };

                if (userRoles.Contains(role.Name))
                {
                    assignRoleToUserViewModel.Exist = true;
                }

                roleViewModelList.Add(assignRoleToUserViewModel);
            }

            return View(roleViewModelList);
        }

        [Authorize(Roles ="role-action")]
        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string userId, List<AssignRoleToUserViewModel> requestList)
        {
            var userToAssignRoles = await _userManager.FindByIdAsync(userId);

            foreach (var role in requestList)
            {
                if (role.Exist)
                {
                    await _userManager.AddToRoleAsync(userToAssignRoles, role.Name);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(userToAssignRoles, role.Name);
                }
            }

            return RedirectToAction(nameof(HomeController.UserList),"Home");
        }
    }
}
