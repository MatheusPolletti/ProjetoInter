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

    [HttpGet]
    public async Task<IActionResult> Buscar(string termo)
    {
        var query = _context.Funcionarios
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