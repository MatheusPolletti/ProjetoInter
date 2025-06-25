using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ProjetoInter.Models;
using Microsoft.AspNetCore.Hosting;

[Authorize]
public class InstituicaoController : BaseController
{
    private readonly IWebHostEnvironment _hostingEnvironment;

    public InstituicaoController(DbZoologico context, IWebHostEnvironment hostingEnvironment) : base(context)
    {
        _hostingEnvironment = hostingEnvironment;
    }

    [HttpGet]
    public async Task<IActionResult> Instituicoes(string busca)
    {
        var query = context.Instituicoes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            busca = busca.ToLower();
            query = query.Where(i => i.Nome.ToLower().Contains(busca) ||
                                     i.Endereco.ToLower().Contains(busca));
        }

        var instituicoes = await query.OrderBy(i => i.Nome).ToListAsync();
        return View(instituicoes);
    }

   [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarInstituicao([FromForm] Instituicao model, [FromForm] IFormFile Imagem)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(model.Nome))
                return BadRequest(new { success = false, message = "Nome é obrigatório" });

            if (string.IsNullOrWhiteSpace(model.Endereco))
                return BadRequest(new { success = false, message = "Endereço é obrigatório" });

            if (Imagem != null && Imagem.Length > 0)
            {
                var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "img", "Instituicao");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Imagem.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Imagem.CopyToAsync(stream);
                }

                model.ImagemUrl = "/img/Instituicao/" + fileName;
            }

            context.Instituicoes.Add(model);
            await context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = "Instituição cadastrada com sucesso!",
                instituicao = new {
                    model.InstituicaoId,
                    model.Nome
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                success = false, 
                message = "Erro interno: " + ex.Message 
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var instituicao = await context.Instituicoes.FindAsync(id);
        if (instituicao == null) return NotFound();

        return PartialView("_ModalEditarInstituicao", instituicao);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarInstituicao(Instituicao model, IFormFile Imagem)
    {
        var erros = new List<string>();

        try
        {
            var instituicaoExistente = await context.Instituicoes.FindAsync(model.InstituicaoId);
            if (instituicaoExistente == null)
            {
                erros.Add("Instituição não encontrada");
                return Json(new { success = false, errors = erros });
            }

            instituicaoExistente.Nome = model.Nome;
            instituicaoExistente.Endereco = model.Endereco;
            instituicaoExistente.Contato = model.Contato;

            if (Imagem != null && Imagem.Length > 0)
            {
                var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "img", "Instituicao");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Imagem.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Imagem.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(instituicaoExistente.ImagemUrl))
                {
                    var oldFilePath = Path.Combine(
                        _hostingEnvironment.WebRootPath,
                        instituicaoExistente.ImagemUrl.TrimStart('/')
                    );

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                instituicaoExistente.ImagemUrl = "/img/Instituicao/" + fileName;
            }

            context.Update(instituicaoExistente);
            await context.SaveChangesAsync();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            erros.Add($"Erro ao atualizar: {ex.Message}");
            return Json(new { success = false, errors = erros });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirInstituicao(int id)
    {
        try
        {
            var instituicao = await context.Instituicoes.FindAsync(id);
            if (instituicao == null)
            {
                return Json(new { success = false, message = "Instituição não encontrada." });
            }

            var funcionariosAssociados = await context.Funcionarios
                .AnyAsync(f => f.InstituicaoId == id);

            if (funcionariosAssociados)
            {
                return Json(new { 
                    success = false, 
                    message = "Não é possível excluir a instituição pois existem funcionários associados a ela." 
                });
            }

            var transferenciasAssociadas = await context.Transferencias
                .AnyAsync(t => t.InstituicaoOrigemId == id || t.InstituicaoDestinoId == id);

            if (transferenciasAssociadas)
            {
                return Json(new { 
                    success = false, 
                    message = "Não é possível excluir a instituição pois existem transferências associadas a ela." 
                });
            }

            if (!string.IsNullOrEmpty(instituicao.ImagemUrl))
            {
                var filePath = Path.Combine(
                    _hostingEnvironment.WebRootPath,
                    instituicao.ImagemUrl.TrimStart('/')
                );

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            context.Instituicoes.Remove(instituicao);
            await context.SaveChangesAsync();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodasInstituicoes()
    {
        var instituicoes = await context.Instituicoes
            .OrderBy(i => i.Nome)
            .ToListAsync();

        return Ok(instituicoes);
    }
}