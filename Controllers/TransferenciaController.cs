using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class TransferenciaController : Controller
{
    private readonly DbZoologico context;

    public TransferenciaController(DbZoologico _context)
    {
        context = _context;
    }

    public IActionResult Transferencias()
    {
        return View();
    }
}