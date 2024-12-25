using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using SupportTicketApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SupportTicketApp.Context;
using SupportTicketApp.Utils;

var builder = WebApplication.CreateBuilder(args);
// Session
builder.Services.AddDistributedMemoryCache();  // Oturum verilerini bellekte saklar
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Oturum süresi
    options.Cookie.HttpOnly = true;  // Güvenlik için HttpOnly 
    options.Cookie.IsEssential = true;  // Çerez her durumda kullanýlabilir olmalý
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// EmailService sýnýfýný DI container'ýna ekliyoruz
builder.Services.AddTransient<EmailService>();

builder.Services.AddDbContext<SupportTicketDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Baðlantý dizesi ismi
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.Cookie.HttpOnly = true;  // HttpOnly ile çerezin JavaScript'ten eriþilmesi engellenir.
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;  // HTTPS üzerinden gönderilmesi saðlanýr.
                options.SlidingExpiration = true;  // Kullanýcý etkinlik gösterirse çerez süresi yenilenir.
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Oturumun süresi 30 dakika.
                options.LogoutPath = "/Account/Logout"; // Çýkýþ
                options.Cookie.IsEssential = true;  // Çerez oturum çerezi olur ve tarayýcý kapandýðýnda silinir.
                options.Cookie.MaxAge = null;  // Ensure it doesn't persist (i.e., expires with the browser session).
            });
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<SupportTicketDbContext>();
        //var signInManager = services.GetRequiredService<SignInManager<IdentityUser>>();

        var accountController = new AccountController(context);

        var result = await accountController.CreateAdminUser() as JsonResult;  

    }
    catch (Exception ex)
    {
        Console.WriteLine("Admin kullanýcý oluþturulamadý: " + ex.Message);
    }
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
app.UseSession();  // Session kullanýmýný etkinleþtirme

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;
    if (response.StatusCode == StatusCodes.Status403Forbidden)
    {
        await response.WriteAsync("Bu iþlem için yetkiniz yok.");
    }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

//app.Run("http://0.0.0.0:8000");  // Yerel IP üzerinden baþlat
app.Run();