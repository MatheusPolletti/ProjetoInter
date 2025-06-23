using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using ProjetoInter.Models;
using System.Threading.Tasks; // Adicione este using para Task

[Authorize]
public class TransferenciaController : BaseController
{
    private readonly DbZoologico _context; // Use _context para consistência

    public TransferenciaController(DbZoologico context) : base(context)
    {
        _context = context; // Atribua o contexto injetado
    }

    // Renomeie esta Action de 'Index' para 'Transferencias'
    [HttpGet] // A requisição do formulário de busca é GET
    public async Task<IActionResult> Transferencias(string busca) // <--- MUDANÇA AQUI: Nome da Action para Transferencias
    {
        // Inicia a query com todas as transferências e inclui as entidades relacionadas
        var query = _context.Transferencias // Use _context aqui
                           .Include(t => t.Animal)
                               .ThenInclude(a => a.Especie)
                           .Include(t => t.Animal)
                               .ThenInclude(a => a.Setor)
                           .Include(t => t.InstituicaoOrigem)
                           .Include(t => t.InstituicaoDestino)
                           .AsQueryable();

        // Aplica o filtro de busca se um termo for fornecido
        if (!string.IsNullOrWhiteSpace(busca))
        {
            busca = busca.ToLower();

            query = query.Where(t => t.Animal.Nome.ToLower().Contains(busca) ||
                                     t.InstituicaoOrigem.Nome.ToLower().Contains(busca) ||
                                     t.InstituicaoDestino.Nome.ToLower().Contains(busca));
        }

        query = query.OrderByDescending(t => t.DataSaida);

        var transferencias = await query.ToListAsync();

        // Carregar dados para os dropdowns dos modais
        await CarregarDadosParaDropdowns(); // Chama o método auxiliar

        // Retorna a view 'Transferencias.cshtml' passando a lista filtrada como Model
        return View(transferencias); // <--- MUDANÇA AQUI: Passando o Model para a View
    }

    // Método auxiliar para carregar os dados dos dropdowns
    private async Task CarregarDadosParaDropdowns()
    {
        ViewBag.Animais = await _context.Animais
                                       .Include(a => a.Especie)
                                       .Include(a => a.Setor)
                                       .Where(a => a.StatusId == 1) // Exemplo de filtro: animais ativos
                                       .OrderBy(a => a.Nome)
                                       .ToListAsync();
        ViewBag.Instituicoes = await _context.Instituicoes
                                            .OrderBy(i => i.Nome)
                                            .ToListAsync();
    }


    // SEUS OUTROS MÉTODOS FICAM AQUI: CriarTransferencia, ObterTransferenciaPorId, AtualizarTransferencia, ExcluirTransferencia, ConcluirTransferencia
    // Certifique-se de que eles estão usando _context e não 'context' (erro de cópia e cola)

    // Exemplo do método CriarTransferencia
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
            model.Status = true; // Exemplo: Nova transferência é Pendente por padrão
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

    // Exemplo do método ObterTransferenciaPorId:
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
    
    // Exemplo do método para Atualizar uma transferência
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
            // Não se esqueça de atualizar o status se for relevante aqui, ex:
            // transferenciaExistente.Status = model.Status;

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

    // Exemplo do método para Excluir (remover permanentemente)
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
            Console.WriteLine($"Erro ao excluir transferência: {ex.Message}");
            return StatusCode(500, new { success = false, message = "Erro interno do servidor ao excluir a transferência." });
        }
    }

    // Método para Concluir Transferência
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

            transferencia.Status = false; // Define o status como 'concluído' (false, como você tinha)
            
            // Certifique-se de que DataEntrada seja preenchida ao concluir!
            if (transferencia.DataEntrada == null) 
            {
                transferencia.DataEntrada = DateTime.Now; 
            }

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