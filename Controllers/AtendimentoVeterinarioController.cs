using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class AtendimentoVeterinarioController : BaseController
{
    public AtendimentoVeterinarioController(DbZoologico _context) : base(_context) { }

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
    public IActionResult AtendimentoInfo(int id)
    {
        var atendimento = context.AtendimentosVeterinarios
            .Include(a => a.Animal)                  // Carrega o Animal
                .ThenInclude(a => a.Especie)         // Carrega a Especie do Animal (se necessário)
            .Include(a => a.FuncionarioVeterinario)  // Carrega o Veterinário
            .Include(a => a.FuncionarioSolicitante)  // Carrega o Funcionário Solicitante
            .FirstOrDefault(a => a.AtendimentoVeterinarioId == id);

        if (atendimento == null)
        {
            return NotFound();
        }

        return View(atendimento);
    }

    [HttpPost]
    public IActionResult AcessarTarefa()
    {
        return RedirectToAction("Tarefas", "Tarefa");
    }
}