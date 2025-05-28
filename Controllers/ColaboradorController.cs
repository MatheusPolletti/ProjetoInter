using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
public class ColaboradorController : Controller
{
    private readonly DbZoologico _context;

    public ColaboradorController(DbZoologico context)
    {
        _context = context;
    }

    public async Task<IActionResult> Colaboradores()
    {
        var funcionarios = await _context.Funcionarios
            .Include(f => f.StatusFuncionario)
            .Where(f => f.StatusFuncionarioId == 1)
            .ToListAsync();

        return View(funcionarios);
    }

    [HttpPost]
    public IActionResult AcessarTarefa()
    {
        return RedirectToAction("Tarefas", "Tarefa");
    }
}