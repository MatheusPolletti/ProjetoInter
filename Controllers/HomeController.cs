using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoInter.Data;

public class HomeController : Controller
{
    private readonly DbZoologico _context;

    public HomeController(DbZoologico context)
    {
        _context = context;
    }

    public async Task<IActionResult> LoginCadastro()
    {
        // Aqui puxamos Transferencias e todas as entidades relacionadas via Include
        var transferencias = await _context.Transferencias
            .Include(t => t.Animal)
                .ThenInclude(a => a.Status)
            .Include(t => t.Animal)
                .ThenInclude(a => a.Especie)
            .Include(t => t.Animal)
                .ThenInclude(a => a.Setor)
            .Include(t => t.InstituicaoOrigem)
            .Include(t => t.InstituicaoDestino)
            .ToListAsync();

        // Além disso, para puxar tudo, podemos trazer também outras tabelas:
        var animais = await _context.Animais
            .Include(a => a.Status)
            .Include(a => a.Especie)
            .Include(a => a.Setor)
            .ToListAsync();

        var funcionarios = await _context.Funcionarios.ToListAsync();
        var procedimentos = await _context.Procedimentos
            .Include(p => p.Animal)
            .Include(p => p.FuncionarioTarefa)
            .ToListAsync();

        var setores = await _context.Setores
            .Include(s => s.InstituicaoPertence)
            .ToListAsync();

        var atendimentos = await _context.AtendimentosVeterinarios
            .Include(av => av.Animal)
            .Include(av => av.FuncionarioSolicitante)
            .Include(av => av.FuncionarioVeterinario)
            .ToListAsync();

        // Pode criar um ViewModel para agregar tudo, mas para teste pode enviar via ViewBag
        ViewBag.Transferencias = transferencias;
        ViewBag.Animais = animais;
        ViewBag.Funcionarios = funcionarios;
        ViewBag.Procedimentos = procedimentos;
        ViewBag.Setores = setores;
        ViewBag.Atendimentos = atendimentos;

        return View();
    }
}
