using DataAccess.Context;
using Entities.Concrete;
using WebMVC.CustomValidations;
using WebMVC.Localizations;

namespace WebMVC.Extensions
{
    public static class StartupExtensions
    {
        public static void AddIdentityWithExtension(this IServiceCollection services)
        {
            //Identity ekliyorum. Ayrıca hangi DbContext i kullanacağını ekliyorum.
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true; //Email tek aynı email ile başkası eklenemez.
                options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwxyz1234567890_";


                options.Password.RequiredLength = 6; //şifre 6 karakter olsun
                options.Password.RequireNonAlphanumeric = false; //?* gibi karakter olmasın
                options.Password.RequireDigit = false; //Rakam olması şart olmasın
                options.Password.RequireLowercase = true; //Küçük harf zorunlu olsun
                options.Password.RequireUppercase = false; //Büyük harf zorunlu olmasın

                // Lockout yani hesabı kilitleme ile ilgili ayarlar.

                //Default olarak hesap kilitleme süresi 5dk biz burada 3dk yapıyoruz.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.Lockout.MaxFailedAccessAttempts = 3; //Başarısız giriş sayısını 3 yapıyoruz.

            }).AddPasswordValidator<PasswordValidator>()
              .AddErrorDescriber<LocalizationIdentityErrorDescriber>()
              .AddEntityFrameworkStores<AppDbContext>();

        }
    }
}
