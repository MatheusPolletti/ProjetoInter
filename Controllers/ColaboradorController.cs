using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;

public class ColaboradorController : Controller
{
    private readonly DbZoologico _context;

    public ColaboradorController(DbZoologico context)
    {
        _context = context;
    }

    public IActionResult Colaboradores()
    {
        return View();
    }
    
    
}