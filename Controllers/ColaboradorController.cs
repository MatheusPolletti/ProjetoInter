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

    public async Task<IActionResult> Funcionarios()
    {
        var funcionarios = await _context.Funcionarios
        
        .Include(f => f.StatusFuncionario)
        .Include(f => f.Cargo)
        .Include(f => f.Cpf)
        .Include(f => f.Telefone)
        .Where(f => f.StatusFuncionarioId == 1) // Filtra apenas os colaboradores ativos

        .ToListAsync();

        return View(funcionarios);
    }

     [HttpPost]
    public IActionResult AcessarTarefa()
    {
        return RedirectToAction("Tarefas", "Tarefa");
    }
}
