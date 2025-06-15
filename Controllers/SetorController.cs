using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class SetorController : Controller
{
    private readonly DbZoologico context;

    public SetorController(DbZoologico _context)
    {
        context = _context;
    }

    public async Task<IActionResult> Setores()
    {
        var setores = await context.Setores
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