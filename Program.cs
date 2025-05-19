using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DbZoologico>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession();

var app = builder.Build();

app.MapControllerRoute("default", "/{controller=Home}/{action=LoginCadastro}/{id?}");

app.UseSession();

app.UseStaticFiles();

app.Run();