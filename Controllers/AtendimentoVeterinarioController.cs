using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;

public class AtendimentoVeterinarioController : Controller
{
    private readonly DbZoologico _context;

    public AtendimentoVeterinarioController(DbZoologico context)
    {
        _context = context;
    }

     public IActionResult AtendimentosVeterinarios()
    {
        return View();
    }
}