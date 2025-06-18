using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class ColaboradorController : BaseController
{
    public ColaboradorController(DbZoologico _context) : base(_context) {}

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
    public async Task<IActionResult> Colaboradores(string busca)
<<<<<<< HEAD
{
=======
    {
>>>>>>> 084014b1139905d919488b880cc1883258779381
    var query = context.Funcionarios
        .Include(f => f.StatusFuncionario)
        .Where(f => f.StatusFuncionarioId == 1); // Apenas ativos

    if (!string.IsNullOrEmpty(busca))
    {
        busca = busca.ToLower();
            query = query.Where(f => f.Nome.ToLower().Contains(busca)
                || f.Cpf.ToLower().Contains(busca)
                || f.Cargo.ToLower().Contains(busca));
    }

    var funcionarios = await query.ToListAsync();
    return View(funcionarios);
    }

    [HttpGet]
    public async Task<IActionResult> DetalhesColaborador(int id)
    {
        var funcionario = await context.Funcionarios
            .Include(f => f.StatusFuncionario)
            .FirstOrDefaultAsync(f => f.FuncionarioId == id);

        if (funcionario == null)
        {
            return NotFound();
        }

        return View(funcionario);
    }
}