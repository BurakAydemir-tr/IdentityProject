using System.ComponentModel.DataAnnotations;

namespace WebMVC.Areas.Admin.Models
{
    public class RoleUpdateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Rol isim alanı boş bırakılamaz")]
        [Display(Name = "Role ismi :")]
        public string Name { get; set; } = null!; //null olmayacağını belirtiyoruz.
    }
}
