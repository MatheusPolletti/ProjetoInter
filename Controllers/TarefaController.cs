using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;

public class TarefaController : Controller
{
    private readonly DbZoologico _context;

    public TarefaController(DbZoologico context)
    {
        _context = context;
    }

    public IActionResult Tarefas()
    {
        return View();
    }
}