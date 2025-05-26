using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;

public class AnimalController : Controller
{
    private readonly DbZoologico _context;

    public AnimalController(DbZoologico context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Animais()
    {
        var animais = await _context.Animais
        .Include(a => a.Setor)
        .Include(a => a.Status)
        .Include(a => a.Especie)
        .ToListAsync();

        return View(animais);
    }
    
    [HttpPost]
    public IActionResult AcessarTarefa()
    {
        return RedirectToAction("Tarefas", "Tarefa");
    }
}