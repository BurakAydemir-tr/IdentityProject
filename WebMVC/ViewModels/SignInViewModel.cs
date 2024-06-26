﻿using System.ComponentModel.DataAnnotations;

namespace WebMVC.ViewModels
{
    public class SignInViewModel
    {
        public SignInViewModel()
        {
        }

        public SignInViewModel(string email, string password)
        {
            Email = email;
            Password = password;
        }


        [EmailAddress(ErrorMessage = "Email formatı yanlıştır.")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz.")]
        [Display(Name = "Email :")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz.")]
        [Display(Name = "Şifre :")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter olabilir")]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla ")]
        public bool RememberMe { get; set; }
    }
}
