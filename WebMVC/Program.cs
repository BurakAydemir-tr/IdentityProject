using DataAccess.Context;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using WebMVC.Extensions;
using WebMVC.OptionsModel;
using WebMVC.PermissionsRoot;
using WebMVC.Requirements;
using WebMVC.Seeds;
using WebMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

/*DbContext imi ekledim. Connectionstring i nerden alacaðýný yani appsettings den alacaðýný gösteriyorum*/
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnectionString"));
});

//builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<AppDbContext>();

// Burada EmailSettings class ýndaki propertilerle appsettings deki EmailSettings deðerlerini eþliyoruz.
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddIdentityWithExtension();

//Dosya iþlmeleri için kullanýlýyor. Directory.GetCurrentDirectory diyerek projedeki tüm klasörlere eriþebiliyoruz. Bunun için IFileProvider arayüzünü enjekte etmek gerekiyor.
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpireRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ViolenceRequirementHandler>();

//Authorization ve policy ile ilgili ayarlarý burada yapýyoruz.
builder.Services.AddAuthorization(options =>
{
    //ExchangePolicy adýnda bir policy ekliyoruz. Bu policy için gereken iþ kuralý requirement olarak ekleniyor.
    options.AddPolicy("ExchangePolicy", poicy => 
    {
        poicy.AddRequirements(new ExchangeExpireRequirement());
    });

    //Þiddet içeren içeriklere eriþimi kýsýtlamak için ViolencePolicy ekliyoruz. Bu policy için gereken iþ kuralý requirement olarak ekleniyor. Sýnýr olarak ThresholdAge deðiþkeni tanýmlanýyor ve bu deðiþkene sýnýr yaþý veriliyor.
    options.AddPolicy("ViolencePolicy", poicy =>
    {
        poicy.AddRequirements(new ViolenceRequirement() { ThresholdAge = 18});
    });

    options.AddPolicy("OrderPermissionReadAndDelete", poicy =>
    {
        poicy.RequireClaim("Permission", Permissions.Order.Read);
        poicy.RequireClaim("Permission", Permissions.Order.Delete);
        poicy.RequireClaim("Permission", Permissions.Stock.Delete);
    });

    options.AddPolicy("Permissions.Order.Read", poicy =>
    {
        poicy.RequireClaim("Permission", Permissions.Order.Read);
    });

    options.AddPolicy("Permissions.Order.Delete", poicy =>
    {
        poicy.RequireClaim("Permission", Permissions.Order.Delete);
    });

    options.AddPolicy("Permissions.Stock.Delete", poicy =>
    {
        poicy.RequireClaim("Permission", Permissions.Stock.Delete);
    });
});

// Cookie ayarlarýný burada yapýyoruz.
builder.Services.ConfigureApplicationCookie(opt =>
{
    var cookieBuilder= new CookieBuilder();
    cookieBuilder.Name = "UdemyAppCookie"; // Cookie ye bir isim veriyoruz.
    opt.LoginPath = new PathString("/Home/Signin"); //eriþmemesi gereken yerlere girmeye çalýþtýðýnda login sayfasýna yönlendiriyor.
    opt.LogoutPath = new PathString("/Member/Logout");
    opt.AccessDeniedPath = new PathString("/Member/AccessDenied");
    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan = TimeSpan.FromDays(60); // Cookie nin geçerlilik süresi 60 gün ayarlýyoruz.
    opt.SlidingExpiration = true; // Cookie yi sürekli muhafaza etmek için
});

var app = builder.Build();

// Program ayaða kaltýðýnda bir scope oluþturulacak 
using(var scope = app.Services.CreateScope())
{
    //GetRequiredService metodu ile roleManager ý alýyoruz.
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    //Seed datayý program çalýþýnca çalýþtýrýp veritabanýna kaydediyoruz.
    await PermissionSeed.Seed(roleManager);
}

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
