using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoInter.Data;

namespace ProjetoInter.Controllers;

public class UsuarioController : Controller
{
    private readonly DbZoologico _context;

    public UsuarioController(DbZoologico context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var usuarios = await _context.Usuarios.ToListAsync();
        return View(usuarios);
    }
}