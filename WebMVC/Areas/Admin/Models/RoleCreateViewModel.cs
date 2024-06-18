using System.ComponentModel.DataAnnotations;

namespace WebMVC.Areas.Admin.Models
{
    public class RoleCreateViewModel
    {
        [Required(ErrorMessage ="Rol isim alanı boş bırakılamaz")]
        [Display(Name ="Role ismi :")]
        public string Name { get; set; }
    }
}
