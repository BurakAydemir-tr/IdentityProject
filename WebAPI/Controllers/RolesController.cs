using Entities.Concrete;
using Entities.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RolesController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(AppRoleDto appRoleDto)
        {
            var appRole = await _roleManager.FindByNameAsync(appRoleDto.Name);
            if (appRole == null)
            {
                var result = await _roleManager.CreateAsync(new AppRole
                {
                    Name = appRoleDto.Name,
                    CreatedOn = DateTime.UtcNow,
                });
                if (result.Succeeded)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Bu isimde rol bulunmaktadır.");
        }

        [HttpGet("GetRoles")]
        public IActionResult GetRoles()
        {
            var result= _roleManager.Roles.ToList();
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest("Roller bulunamadı.");
        }

        [HttpPost("RoleAssign")]
        public async Task<IActionResult> RoleAssign(List<RoleAssignDto> roleAssignDtos, string userId)
        {
            var appUser= await _userManager.FindByIdAsync(userId);
            foreach (var role in roleAssignDtos)
            {
                if (role.IsAssign)
                {
                    await _userManager.AddToRoleAsync(appUser, role.RoleName);
                }else
                {
                    await _userManager.RemoveFromRoleAsync(appUser, role.RoleName);
                }
            }
            return Ok("Rol atamsı gerçekleştirildi.");
        }
    }
}
