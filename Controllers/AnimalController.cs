using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;

public class AnimalController : Controller
{
    private readonly DbZoologico _context;

    public AnimalController(DbZoologico context)
    {
        _context = context;
    }

    public IActionResult Animais()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult AcessarTarefa()
    {
        return RedirectToAction("Tarefas", "Tarefa");
    }
}