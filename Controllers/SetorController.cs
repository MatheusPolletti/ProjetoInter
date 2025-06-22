using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data; // Certifique-se de que este namespace está correto para o seu DbContext
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ProjetoInter.Models; // Certifique-se de que a model Setor está aqui

[Authorize] // Se você estiver usando autenticação
public class SetorController : BaseController // Assumindo que você tem um BaseController
{
    private readonly DbZoologico _context;

    public SetorController(DbZoologico context) : base(context) // Construtor
    {
        _context = context;
    }

    public async Task<IActionResult> Setores()
    {
        var setores = await _context.Setores
            .Include(s => s.InstituicaoPertence)
            .OrderBy(s => s.Nome)
            .ToListAsync();
        
        return View(setores);
    }

    [HttpGet]
    public async Task<IActionResult> ObterSetorPorId(int id)
    {
        var setor = await _context.Setores
            .FirstOrDefaultAsync(s => s.SetorId == id);

        if (setor == null)
        {
            return NotFound(new { success = false, message = "Setor não encontrado." });
        }
        return Ok(setor);
    }

    [HttpPost]
   public async Task<IActionResult> CriarSetor([FromBody] Setor model)
    {
        if (model == null)
        {
            return BadRequest(new { success = false, message = "Dados do setor inválidos." });
        }

        try
        {
            model.SetorId = 0;
            var instituicaoIdClaim = User.FindFirst("InstituicaoId");

            if (instituicaoIdClaim == null || !int.TryParse(instituicaoIdClaim.Value, out int instituicaoId))
            {
                return Unauthorized(new { success = false, message = "Não foi possível determinar a instituição do usuário logado. Claim 'InstituicaoId' não encontrado ou inválido." });
            }

            model.InstituicaoPertenceId = instituicaoId; // Atribui o ID da instituição do usuário logado

            _context.Setores.Add(model);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Setor adicionado com sucesso!", setorId = model.SetorId });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar setor: {ex.Message}");
            return StatusCode(500, new { success = false, message = $"Erro interno do servidor ao criar o setor: {ex.Message}" });
        }
    }

    // 4. Método para atualizar um Setor existente (POST)
    [HttpPost]
    public async Task<IActionResult> AtualizarSetor([FromBody] Setor model)
    {
        if (model == null || model.SetorId <= 0)
        {
            return BadRequest(new { success = false, message = "Dados de setor inválidos para atualização." });
        }

        var setorExistente = await _context.Setores.FirstOrDefaultAsync(s => s.SetorId == model.SetorId);

        if (setorExistente == null)
        {
            return NotFound(new { success = false, message = "Setor não encontrado para atualização." });
        }

        try
        {
            setorExistente.Nome = model.Nome;
            setorExistente.Descricao = model.Descricao;
            setorExistente.Status = model.Status;

            _context.Setores.Update(setorExistente);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Setor atualizado com sucesso!" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar setor: {ex.Message}");
            return StatusCode(500, new { success = false, message = $"Erro interno do servidor ao atualizar o setor: {ex.Message}" });
        }
    }
    // 5. Método para "excluir" Setores (marcar como inativo) - (POST)
    // O ideal é não remover fisicamente se houver dados relacionados.
    [HttpPost]
    public async Task<IActionResult> ExcluirSetores([FromBody] int[] ids)
    {
        if (ids == null || ids.Length == 0)
        {
            return BadRequest(new { success = false, message = "Nenhum ID de setor fornecido para exclusão." });
        }

        try
        {
            // Busca todos os setores que correspondem aos IDs fornecidos
            var setoresParaExcluir = await _context.Setores
                .Where(s => ids.Contains(s.SetorId))
                .ToListAsync();

            if (!setoresParaExcluir.Any())
            {
                return NotFound(new { success = false, message = "Nenhum setor encontrado com os IDs fornecidos." });
            }

            // --- MUDANÇA AQUI: REMOÇÃO FÍSICA ---
            _context.Setores.RemoveRange(setoresParaExcluir); // Remove a coleção de setores
            // --- FIM DA MUDANÇA ---
            
            await _context.SaveChangesAsync(); // Salva as mudanças no banco de dados

            return Ok(new { success = true, message = $"{setoresParaExcluir.Count} setor(es) excluído(s) fisicamente com sucesso!" });
        }
        catch (DbUpdateException dbEx)
        {
            // Captura exceções específicas de banco de dados (ex: Foreign Key Constraints)
            Console.WriteLine($"Erro de banco de dados ao excluir setores: {dbEx.Message}");
            // Você pode inspecionar dbEx.InnerException para detalhes mais específicos
            return StatusCode(500, new { success = false, message = $"Erro ao excluir setor(es): Existem dados relacionados a este(s) setor(es) (ex: animais). Não é possível excluir fisicamente. ({dbEx.Message})" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao excluir setores: {ex.Message}");
            return StatusCode(500, new { success = false, message = $"Erro interno do servidor ao excluir setores: {ex.Message}" });
        }
    }
}