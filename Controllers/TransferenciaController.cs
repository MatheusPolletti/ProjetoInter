using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;

public class TransferenciaController : Controller
{
    private readonly DbZoologico _context;

    public TransferenciaController(DbZoologico context)
    {
        _context = context;
    }

     public IActionResult Transferencias()
    {
        return View();
    }
}