using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class AppUserRegisterDto
    {
        [Required(ErrorMessage ="Lütfen kullanıcı adını boş geçmeyiniz.")]
        [Display(Name ="Kullanıcı Adı")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Lütfen emaili boş geçmeyiniz.")]
        [EmailAddress(ErrorMessage ="Lütfen email formatında bir değer giriniz.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lütfen şifreyi boş geçmeyiniz.")]
        [DataType(DataType.Password, ErrorMessage ="Lütfen kurallara göre şifre giriniz.")]
        [Display(Name = "Şifre")]
        public string Password { get; set; }
        public Gender Gender { get; set; }
    }
}
