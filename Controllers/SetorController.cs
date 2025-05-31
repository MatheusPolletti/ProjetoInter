using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;

public class SetorController : Controller
{
    private readonly DbZoologico _context;

    public SetorController(DbZoologico context)
    {
        _context = context;
    }

    public async Task<IActionResult> Setores()
    {
        var setores = await _context.Setores
        .Include(a => a.InstituicaoPertence)
        .ToListAsync();

        return View(setores);
    }

   [HttpPost]
    public IActionResult AcessarTarefa()
    {
        return RedirectToAction("Tarefas", "Tarefa");
    }
}