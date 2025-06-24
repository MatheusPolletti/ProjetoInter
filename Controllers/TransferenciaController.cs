using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using ProjetoInter.Models;
using System.Threading.Tasks;

[Authorize]
public class TransferenciaController : BaseController
{
    private readonly DbZoologico _context;

    public TransferenciaController(DbZoologico context) : base(context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Transferencias(string busca)
    {
        var query = _context.Transferencias
                           .Include(t => t.Animal)
                               .ThenInclude(a => a.Especie)
                           .Include(t => t.Animal)
                               .ThenInclude(a => a.Setor)
                           .Include(t => t.InstituicaoOrigem)
                           .Include(t => t.InstituicaoDestino)
                           .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            busca = busca.ToLower();
            query = query.Where(t => t.Animal.Nome.ToLower().Contains(busca) ||
                                     t.InstituicaoOrigem.Nome.ToLower().Contains(busca) ||
                                     t.InstituicaoDestino.Nome.ToLower().Contains(busca));
        }

        query = query.OrderByDescending(t => t.DataSaida);

        var transferencias = await query.ToListAsync();
        await CarregarDadosParaDropdowns();

        return View(transferencias);
    }

    private async Task CarregarDadosParaDropdowns()
    {
        ViewBag.Animais = await _context.Animais
                                       .Include(a => a.Especie)
                                       .Include(a => a.Setor)
                                       .Where(a => a.StatusId == 1)
                                       .OrderBy(a => a.Nome)
                                       .ToListAsync();
        ViewBag.Instituicoes = await _context.Instituicoes
                                            .OrderBy(i => i.Nome)
                                            .ToListAsync();
    }

    [HttpPost]
    public async Task<IActionResult> CriarTransferencia([FromBody] Transferencia model)
    {
        if (model == null)
        {
            return BadRequest(new { success = false, message = "Dados de transferência inválidos." });
        }

        if (model.AnimalId <= 0 || model.InstituicaoOrigemId <= 0 || model.InstituicaoDestinoId <= 0)
        {
            return BadRequest(new { success = false, message = "Dados obrigatórios faltando ou inválidos." });
        }

        try
        {
            model.Status = true;
            _context.Transferencias.Add(model);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Transferência criada com sucesso!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Erro interno do servidor: {ex.Message}" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ObterTransferenciaPorId(int id)
    {
        var transferencia = await _context.Transferencias
                                         .Include(t => t.Animal)
                                             .ThenInclude(a => a.Especie)
                                         .Include(t => t.Animal)
                                             .ThenInclude(a => a.Setor)
                                         .Include(t => t.InstituicaoOrigem)
                                         .Include(t => t.InstituicaoDestino)
                                         .FirstOrDefaultAsync(t => t.TransferenciaId == id);

        if (transferencia == null)
        {
            return NotFound(new { success = false, message = "Transferência não encontrada." });
        }
        return Ok(transferencia);
    }
    
    [HttpPost]
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
            return StatusCode(500, new { success = false, message = $"Erro interno do servidor: {ex.Message}" });
        }
    }

    [HttpPost]
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

            transferencia.Status = false;
            transferencia.DataEntrada = DateTime.Now;

            // Inativar o animal após a transferência
            var animal = await _context.Animais.FindAsync(transferencia.AnimalId);
            if (animal != null)
            {
                animal.StatusId = 6; // Status "Inativo"
                _context.Animais.Update(animal);
            }

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Transferência concluída com sucesso e animal inativado!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Erro interno do servidor: {ex.Message}" });
        }
    }
}