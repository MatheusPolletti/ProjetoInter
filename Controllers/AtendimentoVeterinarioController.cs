using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ProjetoInter.Models;

[Authorize]
public class AtendimentoVeterinarioController : BaseController
{
    private readonly IWebHostEnvironment _hostingEnvironment;

    public AtendimentoVeterinarioController(DbZoologico _context, IWebHostEnvironment hostingEnvironment) : base(_context)
    {
        _hostingEnvironment = hostingEnvironment;
    }

    [HttpGet]
    public async Task<IActionResult> AtendimentosVeterinarios(string busca)
    {
        var query = context.AtendimentosVeterinarios
            .Include(a => a.Animal)
                .ThenInclude(animal => animal.Setor)
            .Include(a => a.Animal)
                .ThenInclude(animal => animal.Especie)
            .Include(a => a.FuncionarioSolicitante)
            .Include(a => a.FuncionarioVeterinario)
            .AsQueryable();

        // Adicione esta parte para implementar a busca
        if (!string.IsNullOrWhiteSpace(busca))
        {
            busca = busca.ToLower();
            query = query.Where(a => 
                a.Animal.Nome.ToLower().Contains(busca) ||
                a.Descricao.ToLower().Contains(busca) ||
                a.Resultado.ToLower().Contains(busca) ||
                a.FuncionarioSolicitante.Nome.ToLower().Contains(busca) ||
                a.FuncionarioVeterinario.Nome.ToLower().Contains(busca));
        }

        // Ordenação (opcional)
        query = query.OrderByDescending(a => a.Data);

        var atendimentos = await query.ToListAsync();

        // Mantenha os ViewBags para os dropdowns
        ViewBag.Especies = await context.AnimalEspecies
            .OrderBy(e => e.Descricao)
            .ToListAsync();

        ViewBag.Animais = await context.Animais
            .Include(a => a.Especie)
            .Include(a => a.Setor)
            .Where(a => a.StatusId == 1)
            .OrderBy(a => a.Nome)
            .ToListAsync();

        ViewBag.Funcionarios = await context.Funcionarios
            .Include(f => f.StatusFuncionario)
            .Include(f => f.Instituicao)
            .Where(f => f.StatusFuncionarioId == 1)
            .OrderBy(f => f.Nome)
            .ToListAsync();

        ViewBag.Veterinarios = await context.Funcionarios
            .Where(f => f.StatusFuncionarioId == 1)
            .OrderBy(f => f.Nome)
            .ToListAsync();

        return View(atendimentos);
    }

    public IActionResult AtendimentoInfo(int id)
    {
        var atendimento = context.AtendimentosVeterinarios
            .Include(a => a.Animal)
                .ThenInclude(a => a.Especie)
            .Include(a => a.FuncionarioVeterinario)
            .Include(a => a.FuncionarioSolicitante)
            .FirstOrDefault(a => a.AtendimentoVeterinarioId == id);

        if (atendimento == null)
        {
            return NotFound();
        }

        return View(atendimento);
    }

    [HttpPost]
    public IActionResult CriarAtendimentoAjax([FromBody] AtendimentoVeterinario model)
    {
        if (model == null)
        {
            return BadRequest(new { message = "Dados inválidos." });
        }

        if (string.IsNullOrEmpty(model.Descricao))
        {
            return BadRequest(new { message = "A descrição é obrigatória." });
        }

        try
        {
            model.Status = true;

            context.AtendimentosVeterinarios.Add(model);
            context.SaveChanges();

            return Ok(new { message = "Atendimento solicitado com sucesso!", id = model.AtendimentoVeterinarioId });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao salvar atendimento: {ex.Message}");
            return StatusCode(500, new { message = "Erro interno do servidor ao salvar o atendimento." });
        }
    }

    [HttpPost]
    public IActionResult CriarAtendimento(AtendimentoVeterinario atendimento)
    {
        if (ModelState.IsValid)
        {
            atendimento.Status = true;

            context.AtendimentosVeterinarios.Add(atendimento);
            context.SaveChanges();

            return RedirectToAction("AtendimentosVeterinarios");
        }

        ViewBag.Animais = context.Animais.ToList();
        ViewBag.Funcionarios = context.Funcionarios.ToList();

        return View(atendimento);
    }

    [HttpPost]
    public IActionResult AcessarTarefa()
    {
        return RedirectToAction("Tarefas", "Tarefa");
    }

    [HttpGet("/AtendimentoVeterinario/ObterAtendimentoPorId/{id}")] // Rota convencional: /AtendimentoVeterinario/ObterAtendimentoPorId/{id}
    public async Task<IActionResult> ObterAtendimentoPorId(int id)
    {
        var atendimento = await context.AtendimentosVeterinarios
            .Include(a => a.Animal)
                .ThenInclude(animal => animal.Especie)
            .Include(a => a.Animal)
                .ThenInclude(animal => animal.Setor)
            .Include(a => a.FuncionarioSolicitante)
            .Include(a => a.FuncionarioVeterinario)
            .FirstOrDefaultAsync(a => a.AtendimentoVeterinarioId == id);

        if (atendimento == null)
        {
            return NotFound(new { success = false, message = "Atendimento não encontrado." });
        }

        return Ok(atendimento);
    }

    // MÉTODO PARA ATUALIZAR
    [HttpPost("/AtendimentoVeterinario/AtualizarAtendimento")] // Rota convencional: /AtendimentoVeterinario/AtualizarAtendimento
    public async Task<IActionResult> AtualizarAtendimento([FromBody] AtendimentoVeterinario model)
    {
        if (model == null || model.AtendimentoVeterinarioId <= 0)
        {
            return BadRequest(new { success = false, message = "Dados de atendimento inválidos." });
        }

        var atendimentoExistente = await context.AtendimentosVeterinarios.FirstOrDefaultAsync(a => a.AtendimentoVeterinarioId == model.AtendimentoVeterinarioId);

        if (atendimentoExistente == null)
        {
            return NotFound(new { success = false, message = "Atendimento não encontrado para atualização." });
        }

        try
        {
            atendimentoExistente.AnimalId = model.AnimalId;
            atendimentoExistente.FuncionarioSolicitanteId = model.FuncionarioSolicitanteId;
            atendimentoExistente.FuncionarioVeterinarioId = model.FuncionarioVeterinarioId;
            atendimentoExistente.Data = model.Data;
            atendimentoExistente.Descricao = model.Descricao;
            atendimentoExistente.Resultado = model.Resultado;
            atendimentoExistente.Observacoes = model.Observacoes;
            atendimentoExistente.Status = model.Status;

            context.AtendimentosVeterinarios.Update(atendimentoExistente);
            await context.SaveChangesAsync();

            return Ok(new { success = true, message = "Atendimento atualizado com sucesso!" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar atendimento: {ex.Message}");
            return StatusCode(500, new { success = false, message = "Erro interno do servidor ao atualizar o atendimento." });
        }
    }

    [HttpPost("/AtendimentoVeterinario/ConcluirAtendimento")]
    public async Task<IActionResult> ConcluirAtendimento([FromBody] AtendimentoVeterinario model) // Mude o tipo do parâmetro
    {
        if (model == null || model.AtendimentoVeterinarioId <= 0)
        {
            return BadRequest(new { success = false, message = "Dados de atendimento inválidos." });
        }

        var atendimentoExistente = await context.AtendimentosVeterinarios.FirstOrDefaultAsync(a => a.AtendimentoVeterinarioId == model.AtendimentoVeterinarioId);

        if (atendimentoExistente == null)
        {
            return NotFound(new { success = false, message = "Atendimento não encontrado para conclusão." });
        }

        try
        {
            // Atualize os campos relevantes do atendimento existente
            atendimentoExistente.Resultado = model.Resultado; // Exemplo: atualiza resultado
            atendimentoExistente.Observacoes = model.Observacoes; // Exemplo: atualiza observações
            atendimentoExistente.Status = false; // Define o status como false para "concluir"

            context.AtendimentosVeterinarios.Update(atendimentoExistente);
            await context.SaveChangesAsync();

            return Ok(new { success = true, message = "Atendimento concluído e atualizado com sucesso!" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao concluir atendimento: {ex.Message}");
            return StatusCode(500, new { success = false, message = "Erro interno do servidor ao concluir o atendimento." });
        }
    }

    [HttpPost("/AtendimentoVeterinario/ExcluirAtendimento")]
    public async Task<IActionResult> ExcluirAtendimento([FromBody] int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { success = false, message = "ID de atendimento inválido." });
        }

        try
        {
            var atendimento = await context.AtendimentosVeterinarios.FirstOrDefaultAsync(a => a.AtendimentoVeterinarioId == id);

            if (atendimento == null)
            {
                return NotFound(new { success = false, message = "Atendimento não encontrado para exclusão." });
            }

            context.AtendimentosVeterinarios.Remove(atendimento); // Remove o registro
            await context.SaveChangesAsync();

            return Ok(new { success = true, message = "Atendimento excluído permanentemente com sucesso!" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao excluir atendimento: {ex.Message}");
            return StatusCode(500, new { success = false, message = "Erro interno do servidor ao excluir o atendimento." });
        }
    }
}