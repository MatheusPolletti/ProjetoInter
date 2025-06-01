using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;

public class AtendimentoVeterinarioController : Controller
{
    private readonly DbZoologico _context;

    public AtendimentoVeterinarioController(DbZoologico context)
    {
        _context = context;
    }

    public async Task<IActionResult> AtendimentosVeterinarios()
    {
        var atendimentos = await _context.AtendimentosVeterinarios
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