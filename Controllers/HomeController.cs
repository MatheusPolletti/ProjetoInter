using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoInter.Data;

public class HomeController : Controller
{
    private readonly DbZoologico context;

    public HomeController(DbZoologico _context)
    {
        context = _context;
    }

    //public IActionResult Index()
    //{
     //   return View("LoginCadastro");
    //}

    public async Task<IActionResult> LoginCadastro()
    {
        var usuarios = await context.Usuarios.ToListAsync();
        return View(usuarios);  
    }
}