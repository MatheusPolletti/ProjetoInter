using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProjetoInter.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ProjetoInter.Controllers
{
    [Authorize]
    public class TarefaController : BaseController
    {
        public TarefaController(DbZoologico context) : base(context) { }

        [HttpPost]
        public IActionResult AcessarTarefa()
        {
            var funcionario = ObterFuncionarioLogado();
            if (funcionario == null)
            {
                TempData["Erro"] = "Funcionário não encontrado.";
                return RedirectToAction("LoginCadastro", "Home");
            }

            return RedirectToAction("Tarefas");
        }

        [HttpGet] // Adicione este atributo
        public async Task<IActionResult> Tarefas(string busca) // Adicione o parâmetro busca aqui
        {
            var funcionario = ObterFuncionarioLogado();
            if (funcionario == null)
            {
                TempData["Erro"] = "Funcionário não encontrado.";
                return RedirectToAction("LoginCadastro", "Home");
            }

            var query = context.Procedimentos
                .Include(p => p.Animal)
                .ThenInclude(a => a.Especie)
                .Include(p => p.FuncionarioTarefa)
                .Where(p => p.FuncionarioTarefaId == funcionario.FuncionarioId)
                .AsQueryable();

            // Adiciona a busca se o parâmetro foi fornecido
            if (!string.IsNullOrWhiteSpace(busca))
            {
                busca = busca.ToLower();
                query = query.Where(p => 
                    p.Descricao.ToLower().Contains(busca) ||
                    (p.Animal != null && p.Animal.Nome.ToLower().Contains(busca))
                );
            }

            var tarefas = await query.ToListAsync();

            ViewBag.Animais = await context.Animais
                .Include(a => a.Especie)
                .ToListAsync();

            return View(tarefas);
        }

        [HttpPost]
        public async Task<IActionResult> ExcluirTarefa(int id)
        {
            try
            {
                var tarefa = await context.Procedimentos.FindAsync(id);
                if (tarefa == null)
                {
                    return Json(new { success = false, message = "Tarefa não encontrada." });
                }

                context.Procedimentos.Remove(tarefa);
                await context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarTarefa([FromBody] Procedimento model)
        {
            try
            {
                var funcionario = ObterFuncionarioLogado();
                if (funcionario == null)
                {
                    return Unauthorized(new { success = false, message = "Funcionário não autenticado" });
                }

                if (model.AnimalId <= 0)
                {
                    return BadRequest(new { success = false, message = "Animal inválido." });
                }

                var tarefa = new Procedimento
                {
                    Descricao = model.Descricao,
                    AnimalId = model.AnimalId,
                    DataProcedimento = model.DataProcedimento,
                    Observacoes = model.Observacoes,
                    Status = false,
                    FuncionarioTarefaId = funcionario.FuncionarioId
                };

                context.Procedimentos.Add(tarefa);
                await context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    message = "Tarefa criada com sucesso!",
                    tarefaId = tarefa.ProcedimentoId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {
                    success = false,
                    message = "Erro interno",
                    error = ex.Message
                });
            }
        }

        [HttpGet("Tarefa/Editar/{id}")]
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                var tarefa = await context.Procedimentos
                    .Include(p => p.Animal)
                    .ThenInclude(a => a.Especie)
                    .Include(p => p.FuncionarioTarefa)
                    .FirstOrDefaultAsync(p => p.ProcedimentoId == id);

                if (tarefa == null)
                {
                    return NotFound();
                }

                ViewBag.Animais = await context.Animais
                    .Include(a => a.Especie)
                    .Where(a => a.StatusId >= 1 && a.StatusId <= 4)
                    .ToListAsync();

                return PartialView("_ModalEditarTarefa", tarefa);
            }
            catch (Exception)
            {
                return StatusCode(500, "Erro interno ao carregar tarefa");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditarTarefa(Procedimento model)
        {
            try
            {
                var tarefa = await context.Procedimentos.FindAsync(model.ProcedimentoId);
                if (tarefa == null)
                {
                    return Json(new { success = false, message = "Tarefa não encontrada." });
                }

                tarefa.Descricao = model.Descricao;
                tarefa.Observacoes = model.Observacoes;
                tarefa.AnimalId = model.AnimalId;
                tarefa.DataProcedimento = model.DataProcedimento;
                tarefa.Status = model.Status;

                context.Procedimentos.Update(tarefa);
                await context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConcluirTarefa([FromBody] int id)
        {
            try
            {
                var tarefa = await context.Procedimentos.FindAsync(id);
                if (tarefa == null)
                {
                    return Json(new { success = false, message = "Tarefa não encontrada." });
                }

                tarefa.Status = true;
                context.Procedimentos.Update(tarefa);
                await context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirTarefas([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return Json(new { success = false, message = "Nenhuma tarefa selecionada" });
                }

                var tarefas = await context.Procedimentos
                    .Where(p => ids.Contains(p.ProcedimentoId))
                    .ToListAsync();

                if (tarefas.Count == 0)
                {
                    return Json(new { success = false, message = "Nenhuma tarefa encontrada para exclusão" });
                }

                context.Procedimentos.RemoveRange(tarefas);
                await context.SaveChangesAsync();

                return Json(new { success = true, count = tarefas.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}