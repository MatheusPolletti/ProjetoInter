using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddDbContext<DbZoologico>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")));

// Autenticação (Cookies)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/LoginCadastro";
        options.AccessDeniedPath = "/Home/LoginCadastro";
    });

var app = builder.Build();

// Middlewares
app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=LoginCadastro}/{id?}");

app.MapControllerRoute(
    name: "resetarSenha",
    pattern: "ResetarSenha",
    defaults: new { controller = "ResetarSenha", action = "Index" });

app.Run();