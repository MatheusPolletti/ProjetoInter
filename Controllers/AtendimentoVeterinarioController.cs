using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class AtendimentoVeterinarioController : Controller
{
    private readonly DbZoologico context;

    public AtendimentoVeterinarioController(DbZoologico _context)
    {
        context = _context;
    }

    public async Task<IActionResult> AtendimentosVeterinarios()
    {
        var atendimentos = await context.AtendimentosVeterinarios
        .Include(a => a.Animal)
            .ThenInclude(animal => animal.Setor)
        .Include(a => a.Animal)
            .ThenInclude(animal => animal.Especie)
        .Include(a => a.FuncionarioSolicitante)
        .Include(a => a.FuncionarioVeterinario)
        .ToListAsync();

        return View(atendimentos);
    }

    [HttpPost]
    public IActionResult AcessarTarefa()
    {
        return RedirectToAction("Tarefas", "Tarefa");
    }
}