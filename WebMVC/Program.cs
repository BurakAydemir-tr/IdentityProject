using DataAccess.Context;
using Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using WebMVC.Extensions;
using WebMVC.OptionsModel;
using WebMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

/*DbContext imi ekledim. Connectionstring i nerden alaca��n� yani appsettings den alaca��n� g�steriyorum*/
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnectionString"));
});

//builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<AppDbContext>();

// Burada EmailSettings class �ndaki propertilerle appsettings deki EmailSettings de�erlerini e�liyoruz.
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddIdentityWithExtension();

//Dosya i�lmeleri i�in kullan�l�yor. Directory.GetCurrentDirectory diyerek projedeki t�m klas�rlere eri�ebiliyoruz. Bunun i�in IFileProvider aray�z�n� enjekte etmek gerekiyor.
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));

builder.Services.AddScoped<IEmailService, EmailService>();

// Cookie ayarlar�n� burada yap�yoruz.
builder.Services.ConfigureApplicationCookie(opt =>
{
    var cookieBuilder= new CookieBuilder();
    cookieBuilder.Name = "UdemyAppCookie"; // Cookie ye bir isim veriyoruz.
    opt.LoginPath = new PathString("/Home/Signin"); //eri�memesi gereken yerlere girmeye �al��t���nda login sayfas�na y�nlendiriyor.
    opt.LogoutPath = new PathString("/Member/Logout");
    opt.AccessDeniedPath = new PathString("/Member/AccessDenied");
    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan = TimeSpan.FromDays(60); // Cookie nin ge�erlilik s�resi 60 g�n ayarl�yoruz.
    opt.SlidingExpiration = true; // Cookie yi s�rekli muhafaza etmek i�in
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
