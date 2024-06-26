﻿using System.ComponentModel.DataAnnotations;

namespace WebMVC.ViewModels
{
    public class ResetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz.")]
        [Display(Name = "Yeni Şifre :")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Şifre eşleşmiyor.")]
        [Required(ErrorMessage = "Şifre Tekrar alanı boş bırakılamaz.")]
        [Display(Name = "Yeni Şifre Tekrar :")]
        public string PasswordConfirm { get; set; }
    }
}
