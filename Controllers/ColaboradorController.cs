using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class ColaboradorController : Controller
{
    private readonly DbZoologico context;

    public ColaboradorController(DbZoologico _context)
    {
        context = _context;
    }

    public async Task<IActionResult> Colaboradores()
    {
        var funcionarios = await context.Funcionarios
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

    [HttpGet]
    public async Task<IActionResult> Buscar(string termo)
    {
        var query = context.Funcionarios
            .Include(f => f.StatusFuncionario)
            .Where(f => f.StatusFuncionarioId == 1);

        if (!string.IsNullOrEmpty(termo))
        {
            termo = termo.ToLower();
            query = query.Where(f => f.Nome.ToLower().Contains(termo));
        }

        var resultado = await query.ToListAsync();
        return View("Colaboradores", resultado);
    }
}