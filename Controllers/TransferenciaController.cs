using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using ProjetoInter.Models;

[Authorize]
public class TransferenciaController : BaseController
{
    private readonly DbZoologico _context;

    public TransferenciaController(DbZoologico context) : base(context)
    {
        _context = context;
    }

    public async Task<IActionResult> Transferencias()
    {
        var todasTransferencias = await _context.Transferencias
            .Include(t => t.Animal)
                .ThenInclude(animal => animal.Setor)
            .Include(t => t.Animal)
                .ThenInclude(animal => animal.Especie)
            .Include(t => t.InstituicaoOrigem)
            .Include(t => t.InstituicaoDestino)
            .OrderByDescending(t => t.DataSaida)
            .ToListAsync();

        // --- MUDANÇA AQUI: FILTRA AS TRANSFERÊNCIAS POR STATUS ---
        // Assumindo que Status == true significa "pendente" e Status == false significa "concluído"
        ViewBag.TransferenciasPendentes = todasTransferencias.Where(t => t.Status == true).ToList();
        ViewBag.TransferenciasConcluidas = todasTransferencias.Where(t => t.Status == false).ToList();

        ViewBag.Animais = await _context.Animais
            .Include(a => a.Especie)
            .Include(a => a.Setor)
            .Where(a => a.StatusId == 1) // Supondo que 1 seja para animais ativos
            .OrderBy(a => a.Nome)
            .ToListAsync();

        ViewBag.Instituicoes = await _context.Instituicoes
            .OrderBy(i => i.Nome)
            .ToListAsync();

        // Retorna a view sem um modelo específico, pois agora usamos ViewBags para as listas
        // ou, se preferir manter o @model IEnumerable<Transferencia>, pode retornar ViewBag.TransferenciasPendentes
        return View(); // ou return View(ViewBag.TransferenciasPendentes); se quiser que o `Model` da view seja as pendentes.
                       // Para manter o que você tinha e evitar quebrar outros lugares que possam usar `Model`,
                       // manter `return View(ViewBag.TransferenciasPendentes);` é mais seguro.
                       // Ou idealmente, crie um ViewModel para a página.
    }
    [HttpPost]
    public async Task<IActionResult> CriarTransferencia([FromBody] Transferencia model)
    {
        if (model == null)
        {
            return BadRequest(new { success = false, message = "Dados de transferência inválidos." });
        }

        // Validação adicional (ex: verificar se IDs existem, datas são válidas)
        // A validação agora deve ser para InstituicaoOrigemId e InstituicaoDestinoId
        if (model.AnimalId <= 0 || model.InstituicaoOrigemId <= 0 || model.InstituicaoDestinoId <= 0
        // || model.DataSaida == default(DateOnly)
        )
        {
            return BadRequest(new { success = false, message = "Dados obrigatórios faltando ou inválidos." });
        }

        try
        {
            _context.Transferencias.Add(model);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Transferência criada com sucesso!" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar transferência: {ex.Message}");
            return StatusCode(500, new { success = false, message = $"Erro interno do servidor ao criar a transferência: {ex.Message}" });
        }
    }

    // Adicione aqui métodos para Editar, Excluir, etc. para Transferências
    // Exemplo de como seria o método para obter uma transferência por ID para edição:
    [HttpGet] // Rota convencional: /Transferencia/ObterTransferenciaPorId/{id}
    public async Task<IActionResult> ObterTransferenciaPorId(int id)
    {
        var transferencia = await _context.Transferencias
            .Include(t => t.Animal)
                .ThenInclude(animal => animal.Especie)
            .Include(t => t.Animal)
                .ThenInclude(animal => animal.Setor)
            .Include(t => t.InstituicaoOrigem)
            .Include(t => t.InstituicaoDestino)
            .FirstOrDefaultAsync(t => t.TransferenciaId == id);

        if (transferencia == null)
        {
            return NotFound(new { success = false, message = "Transferência não encontrada." });
        }

        return Ok(transferencia);
    }

    // Exemplo de como seria o método para Atualizar uma transferência
    [HttpPost] // Rota convencional: /Transferencia/AtualizarTransferencia
    public async Task<IActionResult> AtualizarTransferencia([FromBody] Transferencia model)
    {
        if (model == null || model.TransferenciaId <= 0)
        {
            return BadRequest(new { success = false, message = "Dados de transferência inválidos." });
        }

        var transferenciaExistente = await _context.Transferencias.FirstOrDefaultAsync(t => t.TransferenciaId == model.TransferenciaId);

        if (transferenciaExistente == null)
        {
            return NotFound(new { success = false, message = "Transferência não encontrada para atualização." });
        }

        try
        {
            transferenciaExistente.AnimalId = model.AnimalId;
            transferenciaExistente.InstituicaoOrigemId = model.InstituicaoOrigemId;
            transferenciaExistente.InstituicaoDestinoId = model.InstituicaoDestinoId;
            transferenciaExistente.DataSaida = model.DataSaida;
            transferenciaExistente.DataEntrada = model.DataEntrada;

            _context.Transferencias.Update(transferenciaExistente);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Transferência atualizada com sucesso!" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar transferência: {ex.Message}");
            return StatusCode(500, new { success = false, message = $"Erro interno do servidor ao atualizar a transferência: {ex.Message}" });
        }
    }

    // Exemplo de como seria o método para Excluir (remover permanentemente)
    [HttpPost] // Rota convencional: /Transferencia/ExcluirTransferencia
    public async Task<IActionResult> ExcluirTransferencia([FromBody] int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { success = false, message = "ID de transferência inválido." });
        }

        try
        {
            var transferencia = await _context.Transferencias.FirstOrDefaultAsync(t => t.TransferenciaId == id);

            if (transferencia == null)
            {
                return NotFound(new { success = false, message = "Transferência não encontrada para exclusão." });
            }

            _context.Transferencias.Remove(transferencia);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Transferência excluída permanentemente com sucesso!" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao excluir transferência: {ex.Message}");
            return StatusCode(500, new { success = false, message = "Erro interno do servidor ao excluir a transferência." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ConcluirTransferencia([FromBody] int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { success = false, message = "ID de transferência inválido." });
        }

        try
        {
            var transferencia = await _context.Transferencias.FirstOrDefaultAsync(t => t.TransferenciaId == id);

            if (transferencia == null)
            {
                return NotFound(new { success = false, message = "Transferência não encontrada." });
            }

            // Define o status como 'concluído' (false)
            transferencia.Status = false;

            _context.Transferencias.Update(transferencia);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Transferência concluída com sucesso!" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao concluir transferência: {ex.Message}");
            return StatusCode(500, new { success = false, message = $"Erro interno do servidor ao concluir a transferência: {ex.Message}" });
        }
    }
}