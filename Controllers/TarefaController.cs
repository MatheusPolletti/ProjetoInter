using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class TarefaController : Controller
{
    private readonly DbZoologico context;

    public TarefaController(DbZoologico _context)
    {
        context = _context;
    }

    public IActionResult Tarefas()
    {
        return View();
    }
}