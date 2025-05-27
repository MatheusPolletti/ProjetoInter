using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;

public class SetorController : Controller
{
    private readonly DbZoologico _context;

    public SetorController(DbZoologico context)
    {
        _context = context;
    }

     public IActionResult Setores()
    {
        return View();
    }
}